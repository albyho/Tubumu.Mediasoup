#!/usr/bin/env bash
set -e

VERSION=${1:-"3.15.7"}
PACKAGE_NAME="Tubumu.Mediasoup.Runtimes"
OUT_DIR="output"
WORK_DIR="build"
BASE_URL="https://github.com/versatica/mediasoup/releases/download/${VERSION}"

# [.NET RID 目录](https://learn.microsoft.com/zh-cn/dotnet/core/rid-catalog)

# 定义平台映射表
# declare -A 是 Bash 4.0+ 的功能
# declare -A PLATFORMS=(
#   ["darwin-arm64"]="osx-arm64"
#   ["darwin-x64"]="osx-x64"
#   ["linux-arm64-kernel6"]="linux-arm64"
#   ["linux-x64-kernel6"]="linux-x64"
#   ["win32-x64"]="win-x64"
# )

# 定义平台映射表（key|value）
PLATFORMS=(
  "darwin-arm64|osx-arm64"
  "darwin-x64|osx-x64"
  "linux-arm64-kernel6|linux-arm64"
  "linux-x64-kernel6|linux-x64"
  "win32-x64|win-x64"
)

echo "➡️ 清理旧构建目录..."
rm -rf "$WORK_DIR/runtimes"
mkdir -p "$WORK_DIR" "$OUT_DIR"

echo "📦 下载并解压文件..."
for entry in "${PLATFORMS[@]}"; do
  key="${entry%%|*}"
  rid="${entry##*|}"
  url="${BASE_URL}/mediasoup-worker-${VERSION}-${key}.tgz"
  tgz_file="$WORK_DIR/$key.tgz"

  if [[ -f "$tgz_file" ]]; then
    echo "✅ 已存在: $tgz_file，跳过下载"
  else
    echo "🔽 下载 $url"
    curl -L "$url" -o "$tgz_file"
  fi

  echo "📂 解压到 runtimes/$rid/native/"
  mkdir -p "$WORK_DIR/runtimes/$rid/native"
  tar -xzf "$tgz_file" -C "$WORK_DIR/runtimes/$rid/native"
done

echo "📝 生成 .nuspec 文件..."
cat > "$WORK_DIR/$PACKAGE_NAME.nuspec" <<EOF
<?xml version="1.0"?>
<package xmlns="http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd">
  <metadata>
    <id>$PACKAGE_NAME</id>
    <version>$VERSION</version>
    <authors>Alby</authors>
    <requireLicenseAcceptance>false</requireLicenseAcceptance>
    <description>Cross-platform mediasoup-worker executables for multiple runtimes.</description>
    <tags>mediasoup native runtimes</tags>
  </metadata>
  <files>
EOF

for entry in "${PLATFORMS[@]}"; do
  key="${entry%%|*}"
  rid="${entry##*|}"
  native_path="runtimes/$rid/native"

  if [[ "$rid" == "win-x64" ]]; then
    echo "    <file src=\"$native_path/mediasoup-worker.exe\" target=\"$native_path/mediasoup-worker.exe\" />" >> "$WORK_DIR/$PACKAGE_NAME.nuspec"
  else
    echo "    <file src=\"$native_path/mediasoup-worker\" target=\"$native_path/mediasoup-worker\" />" >> "$WORK_DIR/$PACKAGE_NAME.nuspec"
  fi
done

echo "  </files>
</package>" >> "$WORK_DIR/$PACKAGE_NAME.nuspec"

echo "📦 构建 NuGet 包..."
pushd "$WORK_DIR" > /dev/null
nuget pack "$PACKAGE_NAME.nuspec" -OutputDirectory "../$OUT_DIR"
popd > /dev/null

echo "✅ 打包完成: $OUT_DIR/${PACKAGE_NAME}.${VERSION}.nupkg"
