#!/usr/bin/env bash
set -e

# 参数
PACKAGE_NAME="Tubumu.Mediasoup.Runtimes"
VERSION=${1:-"3.15.7"}
API_KEY=${2}

if [[ -z "$API_KEY" ]]; then
  echo "❌ 错误: 你需要提供 NuGet API Key。"
  echo "用法: ./publish_nuget.sh 3.15.7 <your-nuget-api-key>"
  exit 1
fi

NUPKG_FILE="output/${PACKAGE_NAME}.${VERSION}.nupkg"

if [[ ! -f "$NUPKG_FILE" ]]; then
  echo "❌ 找不到 NuGet 包: $NUPKG_FILE"
  exit 1
fi

echo "🚀 正在发布 $NUPKG_FILE 到 nuget.org..."

dotnet nuget push "$NUPKG_FILE" \
  --api-key "$API_KEY" \
  --source "https://api.nuget.org/v3/index.json" \
  --skip-duplicate

echo "✅ 发布完成！"
