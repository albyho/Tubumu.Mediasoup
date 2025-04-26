<template>
  <div id="app">
    <el-container>
      <el-header>Demo - 服务模式: {{ serveModeText }}</el-header>
      <el-container>
        <el-aside width="400px">
          <div class="demo-block">
            <el-form ref="connectForm" :inline="true" label-width="60px" size="small">
              <el-form-item label="Peer:">
                <el-select
                  v-model="connectForm.peerId"
                  :disabled="connectForm.isConnected"
                  clearable
                  placeholder="请选择"
                  style="width: 180px"
                >
                  <el-option
                    :label="`Peer ${index}${index == 8 || index === 9 ? '(admin)' : ''}`"
                    v-for="(item, index) in accessTokens"
                    :key="item"
                    :value="index"
                  ></el-option>
                </el-select>
              </el-form-item>
              <el-form-item>
                <el-button type="primary" @click="handleConnect">{{
                  !connectForm.isConnected ? 'Connect' : 'Disconnect'
                }}</el-button>
              </el-form-item>
            </el-form>
          </div>
          <div class="demo-block" v-if="connectForm.isConnected">
            <el-form
              ref="roomForm"
              :model="roomForm"
              :inline="true"
              label-width="60px"
              size="small"
            >
              <el-form-item label="Room:">
                <el-select
                  v-model="roomForm.roomId"
                  :disabled="roomForm.isJoinedRoom"
                  clearable
                  placeholder="请选择"
                  style="width: 180px"
                >
                  <el-option
                    :label="`Room ${index}`"
                    v-for="(item, index) in rooms"
                    :key="item"
                    :value="index"
                  ></el-option>
                </el-select>
              </el-form-item>
              <el-form-item>
                <el-button type="primary" @click="handleJoinRoom">{{
                  !roomForm.isJoinedRoom ? 'Join' : 'Leave'
                }}</el-button>
              </el-form-item>
            </el-form>
          </div>
          <div class="demo-block" v-if="connectForm.isConnected && roomForm.isJoinedRoom">
            <el-form ref="peersForm" :model="peersForm" size="default">
              <el-form-item>
                <el-table
                  ref="singleTable"
                  :data="peersForm.peers"
                  highlight-current-row
                  @current-change="onPeerNodeClick"
                  style="width: 100%"
                >
                  <el-table-column type="index" style="width: 50px"> </el-table-column>
                  <el-table-column property="displayName" label="DisplayName"> </el-table-column>
                </el-table>
              </el-form-item>
            </el-form>
          </div>
        </el-aside>
        <el-main>
          <video
            id="localVideo"
            ref="localVideo"
            v-if="!!camProducer"
            :srcObject="localVideoStream"
            autoplay
            playsinline
          />
          <video
            v-for="[key, value] in remoteVideoStreams"
            :key="key"
            :srcObject="value"
            autoplay
            playsinline
          />
          <audio
            v-for="[key, value] in remoteAudioStreams"
            :key="key"
            :srcObject="value"
            autoplay
          />
        </el-main>
      </el-container>
    </el-container>
  </div>
</template>

<script>
import Logger from './lib/Logger'
import * as mediasoupClient from 'mediasoup-client'
import * as signalR from '@microsoft/signalr'
import { reactive } from "vue"

// eslint-disable-next-line no-unused-vars
const VIDEO_CONSTRAINS = {
  qvga: { width: { ideal: 320 }, height: { ideal: 240 } },
  vga: { width: { ideal: 640 }, height: { ideal: 480 } },
  '720P': { width: { ideal: 1280 }, height: { ideal: 720 } },
  '1080P': { width: { ideal: 1920 }, height: { ideal: 1080 } },
  '4k': { width: { ideal: 4096 }, height: { ideal: 2160 } }
}

const PC_PROPRIETARY_CONSTRAINTS = {
  optional: [{ googDscp: true }]
}

// eslint-disable-next-line no-unused-vars
const WEBCAM_SIMULCAST_ENCODINGS = [
  { scaleResolutionDownBy: 4, maxBitrate: 500000 },
  { scaleResolutionDownBy: 2, maxBitrate: 1000000 },
  { scaleResolutionDownBy: 1, maxBitrate: 5000000 }
]

// Used for VP9 cam video.
// eslint-disable-next-line no-unused-vars
const WEBCAM_KSVC_ENCODINGS = [{ scalabilityMode: 'S3T3_KEY' }]

// Used for simulcast screen sharing.
// eslint-disable-next-line no-unused-vars
const SCREEN_SHARING_SIMULCAST_ENCODINGS = [
  { dtx: true, maxBitrate: 1500000 },
  { dtx: true, maxBitrate: 6000000 }
]

// Used for VP9 screen sharing.
// eslint-disable-next-line no-unused-vars
const SCREEN_SHARING_SVC_ENCODINGS = [{ scalabilityMode: 'S3T3', dtx: true }]

const logger = new Logger('App')

// 'mediasoup-client:* tubumu-meeting-demo-client:*'
localStorage.setItem('debug', 'mediasoup-client:* tubumu-meeting-demo-client:*')

export default {
  name: 'app',
  components: {},
  data() {
    return {
      serveMode: 'Open',
      connection: null,
      mediasoupDevice: null,
      sendTransport: null,
      recvTransport: null,
      nextDataChannelTestNumber: 0,
      cams: new Map(),
      audioDevices: new Map(),
      camProducer: null,
      micProducer: null,
      useSimulcast: false,
      forceH264: false,
      forceVP9: false,
      forceTcp: false,
      localAudioStream: null,
      localVideoStream: null,
      remoteVideoStreams: new Map(),
      remoteAudioStreams: new Map(),
      producers: new Map(),
      consumers: new Map(),
      dataProducer: null,
      dataConsumers: new Map(),
      connectForm: {
        peerId: null,
        isConnected: false
      },
      roomForm: {
        roomId: [],
        isJoinedRoom: false
      },
      peersForm: {
        peers: []
      },
      defaultProps: {
        children: 'children',
        label: 'label'
      },
      form: {
        consume: true,
        produce: true,
        produceAudio: true,
        produceVideo: true,
        useDataChannel: false
      },
      rooms: [
        'Room 0',
        'Room 1',
        'Room 2',
        'Room 3',
        'Room 4',
        'Room 5',
        'Room 6',
        'Room 7',
        'Room 8',
        'Room 9'
      ],
      accessTokens: [
        'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiMCIsIm5iZiI6MTcwNTkzMDcxOSwiZXhwIjoxNzMxODUwNzE5LCJpc3MiOiJpc3N1ZXIiLCJhdWQiOiJhdWRpZW5jZSJ9.O0Oo9CIdtDy3RzAw82J9PMUsw3L8XDw18iQh0-M0Znk',
        'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiMSIsIm5iZiI6MTcwNTkzNDM0NSwiZXhwIjoxNzMxODU0MzQ1LCJpc3MiOiJpc3N1ZXIiLCJhdWQiOiJhdWRpZW5jZSJ9.oZH1fYkLkuCscTOA14fjmyXgkrauEMGUVIgnhcXf5Ic',
        'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiMiIsIm5iZiI6MTcwNTkzNDM3NiwiZXhwIjoxNzMxODU0Mzc2LCJpc3MiOiJpc3N1ZXIiLCJhdWQiOiJhdWRpZW5jZSJ9.qEE4s0sEw0DOf9MuM-TSkKs8ytfgWWWHl01Z3PwyYrk',
        'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiMyIsIm5iZiI6MTcwNTkzNDM5NSwiZXhwIjoxNzMxODU0Mzk1LCJpc3MiOiJpc3N1ZXIiLCJhdWQiOiJhdWRpZW5jZSJ9.2B4uaqobHeXC0bjXJdYK2X-9ktajWgMywrNWCda3MIg',
        'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiNCIsIm5iZiI6MTcwNTkzNDQxMSwiZXhwIjoxNzMxODU0NDExLCJpc3MiOiJpc3N1ZXIiLCJhdWQiOiJhdWRpZW5jZSJ9.V8Lkp5SSv-gTH9-SL-KigU1jJB7U1g5qw6OthqjEnO0',
        'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiNSIsIm5iZiI6MTcwNTkzNDQyNCwiZXhwIjoxNzMxODU0NDI0LCJpc3MiOiJpc3N1ZXIiLCJhdWQiOiJhdWRpZW5jZSJ9.LbYSHbKWfNjw2vhKaOCjWvdqJrbp7JpgtjWclcsRV7o',
        'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiNiIsIm5iZiI6MTcwNTkzNDQzNiwiZXhwIjoxNzMxODU0NDM2LCJpc3MiOiJpc3N1ZXIiLCJhdWQiOiJhdWRpZW5jZSJ9.Fge7PvB8Bdhc9Flj_nFijhp22nYRb9Ei4qvoMQDFMgo',
        'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiNyIsIm5iZiI6MTcwNTkzNDQ0OCwiZXhwIjoxNzMxODU0NDQ4LCJpc3MiOiJpc3N1ZXIiLCJhdWQiOiJhdWRpZW5jZSJ9.xBzsV3skTL7xga64GmRzlIpso1EINESh3vPyFGJznH0',
        'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiOCIsIm5iZiI6MTcwNTkzNDQ2MCwiZXhwIjoxNzMxODU0NDYwLCJpc3MiOiJpc3N1ZXIiLCJhdWQiOiJhdWRpZW5jZSJ9.qfCbpIIslGA18MotXV9DIIQF8Z2IkVUuBfQorZ2l55E',
        'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiOSIsIm5iZiI6MTcwNTkzNDQ3MiwiZXhwIjoxNzMxODU0NDcyLCJpc3MiOiJpc3N1ZXIiLCJhdWQiOiJhdWRpZW5jZSJ9.2WgoUqpVTYRtTctJ8zC0W-1MqjPRIrWHHjX7hH_3t2Q'
      ]
    }
  },
  computed: {
    isAdmin: function () {
      var self = this
      return self.connectForm.peerId === 8 || self.connectForm.peerId === 9
    },
    serveModeText: function () {
      var self = this
      return self.serveMode === 'Open'
        ? '开放(Open)模式'
        : self.serveMode === 'Invite'
          ? '邀请(Invite)模式'
          : '拉取(Pull)模式'
    }
  },
  async mounted() {
    var self = this
    var searchParams = new URLSearchParams(location.search.replace('?', ''))
    self.connectForm.peerId = searchParams.get('peerId') || searchParams.get('peerid')
    if (self.connectForm.peerId) {
      self.connectForm.peerId = parseInt(self.connectForm.peerId)
    }

    self.roomForm.roomId = searchParams.get('roomId') || searchParams.get('roomid')
    if (self.roomForm.roomId) {
      self.roomForm.roomId = parseInt(self.roomForm.roomId)
    }

    // For Testing
    // self.form.produce = self.connectForm.peerId !== '0' && self.connectForm.peerId !== '1';
  },
  methods: {
    async handleConnect() {
      var self = this
      if (self.connectForm.isConnected) {
        if (self.connection) {
          await self.connection.stop()
        }
        self.reset()
        return
      }

      if (!self.connectForm.peerId && self.connectForm.peerId !== 0) {
        self.$message.error('Select a peer, please.')
        return
      }

      try {
        const host = import.meta.env.DEV ? `http://${window.location.hostname}:9000` : ''
        self.connection = new signalR.HubConnectionBuilder()
          .withUrl(`${host}/hubs/meetingHub`, {
            accessTokenFactory: () => self.accessTokens[self.connectForm.peerId],
            skipNegotiation: true,
            transport: signalR.HttpTransportType.WebSockets
          })
          // .withAutomaticReconnect({
          //   nextRetryDelayInMilliseconds: retryContext => {
          //     if (retryContext.elapsedMilliseconds < 60000) {
          //       // If we've been reconnecting for less than 60 seconds so far,
          //       // wait between 0 and 10 seconds before the next reconnect attempt.
          //       return Math.random() * 10000;
          //     } else {
          //       // If we've been reconnecting for more than 60 seconds so far, stop reconnecting.
          //       return null;
          //     }
          //   }
          // })
          .build()

        self.connection.onclose((e) => {
          self.reset()
          if (e) {
            logger.error(e)
          }
        })

        self.connection.on('Notify', async (data) => {
          await self.processNotification(data)
        })
        await self.connection.start()
        await self.start()
        self.connectForm.isConnected = true
      } catch (e) {
        logger.debug(e.message)
      }
    },
    reset() {
      var self = this
      self.connectForm.isConnected = false
      self.roomForm.isJoinedRoom = false
      self.disableMic().catch(() => {})
      self.disableCam().catch(() => {})
      self.closeTransports()
      self.peersForm.peers = []
      self.remoteVideoStreams = reactive(new Map())
      self.remoteAudioStreams = reactive(new Map())
      self.producers = new Map()
      self.dataProducer = null
      self.consumers = new Map()
      self.dataConsumers = new Map()
    },
    async start() {
      var self = this
      let result = await self.connection.invoke('GetServeMode')
      if (result.code !== 200) {
        logger.error(`start() | GetServeMode failed: ${result.message}`)
        self.$message.error(`start() | GetServeMode failed: ${result.message}`)
        return
      }
      self.serveMode = result.data.serveMode

      result = await self.connection.invoke('GetRouterRtpCapabilities')
      if (result.code !== 200) {
        logger.error(`start() | GetRouterRtpCapabilities failed: ${result.message}`)
        self.$message.error(`start() | GetRouterRtpCapabilities failed: ${result.message}`)
        return
      }

      const routerRtpCapabilities = result.data
      self.mediasoupDevice = new mediasoupClient.Device()
      await self.mediasoupDevice.load({
        routerRtpCapabilities
      })

      // NOTE: Stuff to play remote audios due to browsers' new autoplay policy.
      //
      // Just get access to the mic and DO NOT close the mic track for a while.
      // Super hack!
      // {
      //   const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
      //   const audioTrack = stream.getAudioTracks()[0];

      //   audioTrack.enabled = false;

      //   setTimeout(() => audioTrack.stop(), 120000);
      // }

      // GetRouterRtpCapabilities 成功, Join
      result = await self.connection.invoke('Join', {
        rtpCapabilities: self.mediasoupDevice.rtpCapabilities,
        sctpCapabilities:
          self.form.useDataChannel && self.form.consume
            ? self.mediasoupDevice.sctpCapabilities
            : undefined,
        displayName: null,
        sources: ['audio:mic', 'video:cam'],
        appData: {}
      })
      if (result.code !== 200) {
        logger.error(`processNotification() | Join failed: ${result.message}`)
        self.$message.error(`processNotification() | Join failed: ${result.message}`)
        return
      }
    },
    async handleJoinRoom() {
      var self = this
      if (self.roomForm.isJoinedRoom) {
        let result = await self.connection.invoke('LeaveRoom')
        if (result.code !== 200) {
          logger.error(`handleJoinRoom() | LeaveRoom failed: ${result.message}`)
          self.$message.error(`handleJoinRoom() | LeaveRoom failed: ${result.message}`)
          return
        }
        self.peersForm.peers = []
        self.roomForm.isJoinedRoom = false
        await self.disableMic()
        await self.disableCam()
        self.closeTransports()
        return
      }
      if (!self.roomForm.roomId && self.roomForm.roomId !== 0) {
        self.$message.error('Select a room, please.')
        return
      }

      let result = await self.connection.invoke('JoinRoom', {
        roomId: self.roomForm.roomId.toString(),
        role: self.isAdmin ? 'admin' : 'normal'
      })
      if (result.code !== 200) {
        logger.error('handleJoinRoom() | JoinRoom failed.')
        self.$message.error(`handleJoinRoom() | JoinRoom failed: ${result.message}`)
        return
      }

      let { peers } = result.data
      for (let i = 0; i < peers.length; i++) {
        self.peersForm.peers.push(peers[i])
      }
      self.roomForm.isJoinedRoom = true

      if (self.form.produce) {
        // Join成功，CreateSendWebRtcTransport(生产)
        result = await self.connection.invoke('CreateSendWebRtcTransport', {
          forceTcp: self.forceTcp,
          sctpCapabilities: self.form.useDataChannel
            ? self.mediasoupDevice.sctpCapabilities
            : undefined
        })
        if (result.code !== 200) {
          logger.error(`handleJoinRoom() | CreateSendWebRtcTransport failed: ${result.message}`)
          self.$message.error(
            `handleJoinRoom() | CreateSendWebRtcTransport failed: ${result.message}`
          )
          return
        }

        // CreateSendWebRtcTransport(生产)成功, createSendTransport
        self.sendTransport = self.mediasoupDevice.createSendTransport({
          id: result.data.transportId,
          iceParameters: result.data.iceParameters,
          iceCandidates: result.data.iceCandidates,
          dtlsParameters: result.data.dtlsParameters,
          sctpParameters: result.data.sctpParameters,
          iceServers: [],
          proprietaryConstraints: PC_PROPRIETARY_CONSTRAINTS
        })

        self.sendTransport.on('connect', ({ dtlsParameters }, callback, errback) => {
          logger.debug('sendTransport.on() connect dtls: %o', dtlsParameters)
          self.connection
            .invoke('ConnectWebRtcTransport', {
              transportId: self.sendTransport.id,
              dtlsParameters
            })
            .then(callback)
            .catch(errback)
        })

        self.sendTransport.on(
          'produce',
          // appData 需要包含 source
          // eslint-disable-next-line no-unused-vars
          async ({ kind, rtpParameters, appData }, callback, errback) => {
            logger.debug('sendTransport.on() produce, appData: %o', appData)
            try {
              const result = await self.connection.invoke('Produce', {
                transportId: self.sendTransport.id,
                kind,
                rtpParameters,
                source: appData.source,
                appData
              })
              if (result.code !== 200) {
                logger.error(`sendTransport 'produce' callbak | failed: ${result.message}`)
                self.$message.error(`sendTransport 'produce' callbak | failed: ${result.message}`)
                errback(new Error(result.message))
                return
              }
              self.producers.set(result.data.id, result.data)
              callback({ id: result.data.id })
            } catch (error) {
              errback(error)
            }
          }
        )

        self.sendTransport.on(
          'producedata',
          async ({ sctpStreamParameters, label, protocol, appData }, callback, errback) => {
            logger.debug(
              '"producedata" event: [sctpStreamParameters:%o, appData:%o]',
              sctpStreamParameters,
              appData
            )

            try {
              // eslint-disable-next-line no-shadow
              const { id } = await self.connection.invoke('ProduceData', {
                transportId: self.sendTransport.id,
                sctpStreamParameters,
                label,
                protocol,
                appData
              })

              callback({ id })
            } catch (error) {
              errback(error)
            }
          }
        )

        self.sendTransport.on('connectionstatechange', (connectionState) => {
          logger.debug(`sendTransport.on() connectionstatechange: ${connectionState}`)
          if (connectionState === 'connected') {
            self.enableDataProducer()
          }
        })
      }
      // createSendTransport 成功, CreateRecvWebRtcTransport(消费)
      result = await self.connection.invoke('CreateRecvWebRtcTransport', {
        forceTcp: self.forceTcp,
        sctpCapabilities: self.form.useDataChannel
          ? self.mediasoupDevice.sctpCapabilities
          : undefined
      })

      // CreateRecvWebRtcTransport(消费)成功, createRecvTransport
      self.recvTransport = self.mediasoupDevice.createRecvTransport({
        id: result.data.transportId,
        iceParameters: result.data.iceParameters,
        iceCandidates: result.data.iceCandidates,
        dtlsParameters: result.data.dtlsParameters,
        sctpParameters: result.data.sctpParameters,
        iceServers: []
      })

      self.recvTransport.on('connect', ({ dtlsParameters }, callback, errback) => {
        logger.debug('recvTransport.on() connect dtls: %o', dtlsParameters)
        self.connection
          .invoke('ConnectWebRtcTransport', {
            transportId: self.recvTransport.id,
            dtlsParameters
          })
          .then(callback)
          .catch(errback)
      })

      self.recvTransport.on('connectionstatechange', (connectionState) => {
        logger.debug(`recvTransport.on() connectionstatechange: ${connectionState}`)
      })

      if (self.serveMode === 'Open' && self.form.produce) {
        if (self.form.produceAudio) {
          await self.enableMic()
        }
        if (self.form.produceVideo) {
          await self.enableCam()
        }
      }

      if (self.serveMode !== 'Pull') {
        let result = await self.connection.invoke('Ready')
        if (result.code !== 200) {
          logger.error(`Ready() | failed: ${result.message}`)
          self.$message.error(`Ready() | failed: ${result.message}`)
          return
        }
      }
    },
    async onPeerNodeClick(peer) {
      var self = this
      if (!peer) return
      logger.debug('onPeerNodeClick() | %o', peer)
      if (self.serveMode === 'Pull') {
        await self.pull(peer.peerId, peer.sources)
      } else if (self.serveMode === 'Invite') {
        if (self.isAdmin) {
          await self.invite(peer.peerId, peer.sources)
        } else {
          self.$message.error('仅管理员可进行邀请操作。')
        }
      }
    },
    async processNewConsumer(data) {
      var self = this
      const {
        producerPeerId,
        producerId,
        consumerId,
        kind,
        rtpParameters,
        //type, // mediasoup-client 的 Transport.ts 不使用该参数
        producerAppData
        //producerPaused // mediasoup-client 的 Transport.ts 不使用该参数
      } = data

      const consumer = await self.recvTransport.consume({
        id: consumerId,
        producerId,
        kind,
        rtpParameters,
        // producerAppData 中的 peerId 为生产者的 PeerId。
        appData: { ...producerAppData, producerPeerId } // Trick.
      })
      logger.debug('processNewConsumer() Consumer: %o', consumer)

      // Store in the map.
      self.consumers.set(consumer.id, consumer)

      consumer.on('transportclose', () => {
        self.consumers.delete(consumer.id)
      })

      const {
        // eslint-disable-next-line no-unused-vars
        spatialLayers,
        // eslint-disable-next-line no-unused-vars
        temporalLayers
      } = mediasoupClient.parseScalabilityMode(consumer.rtpParameters.encodings[0].scalabilityMode)

      /*
      if (kind === 'audio') {
        consumer.volume = 0;

        const stream = new MediaStream();

        stream.addTrack(consumer.track);

        if (!stream.getAudioTracks()[0]) {
          throw new Error(
            'request.newConsumer | given stream has no audio track'
          );
        }
      }
      */

      const stream = new MediaStream()
      stream.addTrack(consumer.track)

      if (kind === 'video') {
        self.remoteVideoStreams.set(consumerId, stream)
      } else {
        self.remoteAudioStreams.set(consumerId, stream)
      }

      // We are ready. Answer the request so the server will
      // resume this Consumer (which was paused for now).
      logger.debug('processNewConsumer() ResumeConsumer')
      const result = await self.connection.invoke('ResumeConsumer', consumerId)
      if (result.code !== 200) {
        logger.error(`processNewConsumer() | ResumeConsumer failed: ${result.message}`)
        self.$message.error(`processNewConsumer() | ResumeConsumer failed: ${result.message}`)
        return
      }
    },
    async processNewDataConsumer(data) {
      var self = this
      const {
        dataProducerPeerId, // NOTE: Null if bot.
        dataProducerId,
        dataCosumerId,
        sctpStreamParameters,
        label,
        protocol,
        dataProducerAppData
      } = data

      try {
        const dataConsumer = await self.recvTransport.consumeData({
          dataCosumerId,
          dataProducerId,
          sctpStreamParameters,
          label,
          protocol,
          appData: { ...dataProducerAppData, dataProducerPeerId } // Trick.
        })

        // Store in the map.
        self.dataConsumers.set(dataConsumer.id, dataConsumer)

        dataConsumer.on('transportclose', () => {
          self.dataConsumers.delete(dataConsumer.id)
        })

        dataConsumer.on('open', () => {
          logger.debug('DataConsumer "open" event')
        })

        dataConsumer.on('close', () => {
          logger.warn('DataConsumer "close" event')
          self.dataConsumers.delete(dataConsumer.id)
        })

        dataConsumer.on('error', (error) => {
          logger.error('DataConsumer "error" event:%o', error)
        })

        dataConsumer.on('message', (message) => {
          logger.debug(
            'DataConsumer "message" event [streamId:%d]',
            dataConsumer.sctpStreamParameters.streamId
          )

          if (message instanceof ArrayBuffer) {
            const view = new DataView(message)
            const number = view.getUint32()

            if (number == Math.pow(2, 32) - 1) {
              logger.warn('dataChannelTest finished!')
              self.nextDataChannelTestNumber = 0

              return
            }

            if (number > self.nextDataChannelTestNumber) {
              logger.warn(
                'dataChannelTest: %s packets missing',
                number - self.nextDataChannelTestNumber
              )
            }

            self.nextDataChannelTestNumber = number + 1

            return
          } else if (typeof message !== 'string') {
            logger.warn('ignoring DataConsumer "message" (not a string)')
            return
          }

          logger.debug(`New message: ${message}`)
        })
      } catch (error) {
        logger.error('"newDataConsumer" request failed:%o', error)
        throw error
      }
    },
    async pull(peerId, sources) {
      var self = this
      const result = await self.connection.invoke('Pull', {
        peerId,
        sources
      })
      if (result.code !== 200) {
        logger.error(`pull() | failed: ${result.message}`)
        self.$message.error(`pull() | failed: ${result.message}`)
        return
      }
    },
    async invite(peerId, sources) {
      const result = await self.connection.invoke('Invite', {
        peerId,
        sources
      })
      if (result.code !== 200) {
        logger.error(`invite() | failed: ${result.message}`)
        self.$message.error(`invite() | failed: ${result.message}`)
        return
      }
    },
    async processNotification(data) {
      var self = this
      logger.debug('processNotification() | %o', data)
      switch (data.type) {
        case 'newConsumer': {
          await self.processNewConsumer(data.data)

          break
        }

        case 'newDataConsumer': {
          await self.processNewDataConsumer(data.data)

          break
        }

        case 'producerScore': {
          // eslint-disable-next-line no-unused-vars
          const { producerId, score } = data.data

          break
        }

        case 'peerJoinRoom': {
          // eslint-disable-next-line no-unused-vars
          const { peer } = data.data
          self.peersForm.peers.push(peer)
          break
        }

        case 'peerLeaveRoom': {
          // eslint-disable-next-line no-unused-vars
          const { peerId } = data.data
          let idx = -1
          for (let i = 0; i < self.peersForm.peers.length; i++) {
            if (self.peersForm.peers[i].peerId === peerId) {
              idx = i
              break
            }
          }
          if (idx >= 0) {
            self.peersForm.peers.splice(idx, 1)
          }
          break
        }

        case 'peerRoomAppDataChanged': {
          break
        }

        case 'produceSources': {
          if (!self.form.produce) break

          const { /*roomId, */ sources } = data.data
          for (let i = 0; i < sources.length; i++) {
            if (sources[i] === 'audio:mic' && self.mediasoupDevice.canProduce('audio')) {
              if (!self.micProducer) {
                await self.enableMic()
              }
            } else if (sources[i] === 'video:cam' && self.mediasoupDevice.canProduce('video')) {
              if (!self.camProducer) {
                await self.enableCam()
              }
            }
          }

          break
        }

        case 'downlinkBwe': {
          logger.debug("'downlinkBwe' event: %o", data.data)

          break
        }

        case 'consumerClosed': {
          const { consumerId } = data.data
          const consumer = self.consumers.get(consumerId)

          if (!consumer) break

          if (consumer.kind === 'video') {
            self.remoteVideoStreams.delete(consumerId)
          } else {
            self.remoteAudioStreams.delete(consumerId)
          }

          consumer.close()
          self.consumers.delete(consumerId)

          break
        }

        case 'consumerPaused': {
          const { consumerId } = data.data
          const consumer = self.consumers.get(consumerId)

          if (!consumer) break

          break
        }

        case 'consumerResumed': {
          const { consumerId } = data.data
          const consumer = self.consumers.get(consumerId)

          if (!consumer) break

          break
        }

        case 'consumerLayersChanged': {
          // eslint-disable-next-line no-unused-vars
          const { consumerId, spatialLayer, temporalLayer } = data.data
          const consumer = self.consumers.get(consumerId)

          if (!consumer) break

          break
        }

        case 'consumerScore': {
          const { consumerId } = data.data
          const consumer = self.consumers.get(consumerId)

          if (!consumer) break

          break
        }

        case 'producerClosed': {
          const { producerId } = data.data
          const producer = self.producers.get(producerId)

          if (!producer) break

          if (producer.source === 'video:cam') {
            self.camClosed()
          } else if (producer.source === 'audio:mic') {
            self.micClosed()
          }

          break
        }

        case 'peerLeave': {
          const { peerId } = data.data
          for (let i = self.peersForm.peers.length - 1; i > 0; i--) {
            if (self.peersForm.peers[i].peerId === peerId) {
              self.peersForm.peers.splice(i, 1)
              break
            }
          }
          break
        }

        case 'producerVideoOrientationChanged': {
          break
        }

        case 'activeSpeaker': {
          break
        }

        default: {
          logger.error('unknown data.type, data:%o', data)
        }
      }
    },
    async enableDataProducer() {
      var self = this
      logger.debug('enableChatDataProducer()')

      if (!self.form.useDataChannel) return

      try {
        self.dataProducer = await self.sendTransport.produceData({
          ordered: false,
          maxRetransmits: 1,
          label: 'chat',
          priority: 'medium',
          appData: { info: '' }
        })

        self.dataProducer.on('transportclose', () => {
          self.dataProducer = null
        })

        self.dataProducer.on('open', () => {
          logger.debug('DataProducer "open" event')
        })

        self.dataProducer.on('close', () => {
          logger.error('DataProducer "close" event')

          self.dataProducer = null
        })

        self.dataProducer.on('error', (error) => {
          logger.error('chat DataProducer "error" event:%o', error)
        })

        self.dataProducer.on('bufferedamountlow', () => {
          logger.debug('chat DataProducer "bufferedamountlow" event')
        })
      } catch (error) {
        logger.error('enableDataProducer() | failed:%o', error)

        throw error
      }
    },
    async enableMic() {
      var self = this
      logger.debug('enableMic()')

      if (self.micProducer) {
        logger.warn('enableMic() | exists')
        return
      }
      if (self.mediasoupDevice && !self.mediasoupDevice.canProduce('audio')) {
        logger.error('enableMic() | cannot produce audio')
        return
      }

      let track

      try {
        const deviceId = await self._getAudioDeviceId()

        const device = self.audioDevices[deviceId]

        if (!device) throw new Error('no audio devices')

        logger.debug('enableMic() | new selected audio device [device:%o]', device)

        logger.debug('enableMic() | calling getUserMedia()')

        const stream = await navigator.mediaDevices.getUserMedia({
          audio: {
            deviceId: { ideal: deviceId }
          }
        })
        self.localAudioStream = stream

        track = stream.getAudioTracks()[0]

        self.micProducer = await self.sendTransport.produce({
          track,
          codecOptions: {
            opusStereo: 1,
            opusDtx: 1
          },
          appData: { source: 'audio:mic' }
        })

        self.micProducer.on('transportclose', () => {
          self.micProducer = null
        })

        self.micProducer.on('trackended', () => {
          self.disableMic().catch(() => {})
        })
      } catch (error) {
        console.log('enableMic() failed: %o', error)
        logger.error('enableMic() failed: %o', error)
        if (track) track.stop()
      }
    },
    async disableMic() {
      var self = this
      logger.debug('disableMic()')
      if (!self.micProducer) {
        return
      }

      const micProducerId = self.micProducer.id
      self.micClosed()

      try {
        await self.connection.invoke('CloseProducer', micProducerId)
      } catch (error) {
        logger.error('disableMic() [error:"%o"]', error)
      }
    },
    micClosed() {
      var self = this
      if (self.micProducer) {
        self.micProducer.close()
        self.micProducer = null
      }
      self.localAudioStream = null
    },
    async enableCam() {
      var self = this
      logger.debug('enableCam()')

      if (self.camProducer) {
        logger.warn('enableCam() | exists')
        return
      }
      if (self.mediasoupDevice && !self.mediasoupDevice.canProduce('video')) {
        logger.error('enableCam() | cannot produce video')
        return
      }

      let track

      try {
        const deviceId = await self._getCamDeviceId()

        logger.debug(`enableCam() | cam: ${deviceId}`)

        const device = self.cams.get(deviceId)

        if (!device) throw new Error(`no cam devices: ${JSON.stringify(self.cams)}`)

        logger.debug('enableCam() | new selected cam [device:%o]', device)

        logger.debug('enableCam() | calling getUserMedia()')

        //const stream = await navigator.mediaDevices.getUserMedia({ video: true })
        //*
        const stream = await navigator.mediaDevices.getUserMedia({
          video: {
            deviceId: { ideal: deviceId },
            ...VIDEO_CONSTRAINS['720P']
          }
        })
        //*/
        self.localVideoStream = stream

        track = stream.getVideoTracks()[0]

        let encodings
        let codec
        const codecOptions = {
          videoGoogleStartBitrate: 1000
        }

        if (self.forceH264) {
          codec = self.mediasoupDevice.rtpCapabilities.codecs.find(
            (c) => c.mimeType.toLowerCase() === 'video/h264'
          )

          if (!codec) {
            throw new Error('desired H264 codec+configuration is not supported')
          }
        } else if (self.forceVP9) {
          codec = self.mediasoupDevice.rtpCapabilities.codecs.find(
            (c) => c.mimeType.toLowerCase() === 'video/vp9'
          )

          if (!codec) {
            throw new Error('desired VP9 codec+configuration is not supported')
          }
        }

        if (self.useSimulcast) {
          // If VP9 is the only available video codec then use SVC.
          const firstVideoCodec = self.mediasoupDevice.rtpCapabilities.codecs.find(
            (c) => c.kind === 'video'
          )

          if (firstVideoCodec.mimeType.toLowerCase() === 'video/vp9')
            encodings = WEBCAM_KSVC_ENCODINGS
          else encodings = WEBCAM_SIMULCAST_ENCODINGS
        }

        self.camProducer = await self.sendTransport.produce({
          track,
          encodings,
          codecOptions,
          codec,
          appData: { source: 'video:cam' }
        })

        self.camProducer.on('transportclose', () => {
          self.camProducer = null
        })

        self.camProducer.on('trackended', () => {
          self.disableCam().catch(() => {})
        })
        logger.debug('enableCam() succeeded')
      } catch (error) {
        logger.error('enableCam() failed:%o', error)

        if (track) track.stop()
      }
    },
    async disableCam() {
      var self = this
      logger.debug('disableCam()')

      if (!self.camProducer) {
        return
      }

      const camProducerId = self.camProducer.id
      self.camClosed()

      try {
        await self.connection.invoke('CloseProducer', camProducerId)
      } catch (error) {
        logger.error('disableCam() [error:"%o"]', error)
      }
    },
    camClosed() {
      var self = this
      if (self.camProducer) {
        self.camProducer.close()
        self.camProducer = null
      }
      self.localVideoStream = null
    },
    async _updateAudioDevices() {
      var self = this
      logger.debug('_updateAudioDevices()')

      // Reset the list.
      self.audioDevices = {}

      try {
        logger.debug('_updateAudioDevices() | calling enumerateDevices()')

        const devices = await navigator.mediaDevices.enumerateDevices()

        for (const device of devices) {
          if (device.kind !== 'audioinput') continue

          self.audioDevices[device.deviceId] = device
        }
      } catch (error) {
        logger.error('_updateAudioDevices() failed: %o', error)
      }
    },
    async _updateCams() {
      var self = this
      logger.debug('_updateCams()')

      // Reset the list.
      self.cams = new Map()

      try {
        logger.debug('_updateCams() | calling enumerateDevices()')

        const devices = await navigator.mediaDevices.enumerateDevices()

        logger.debug('_updateCams() | %o', devices)
        for (const device of devices) {
          if (device.kind !== 'videoinput') continue
          logger.debug('_updateCams() | %o', device)
          self.cams.set(device.deviceId, device)
        }
      } catch (error) {
        logger.error('_updateCams() failed: %o', error)
      }
    },
    async _getAudioDeviceId() {
      var self = this
      logger.debug('_getAudioDeviceId()')

      try {
        logger.debug('_getAudioDeviceId() | calling _updateAudioDeviceId()')

        await self._updateAudioDevices()

        const audioDevices = Object.values(self.audioDevices)
        return audioDevices[0] ? audioDevices[0].deviceId : null
      } catch (error) {
        logger.error('_getAudioDeviceId() failed: %o', error)
      }
    },
    async _getCamDeviceId() {
      var self = this
      logger.debug('_getCamDeviceId()')

      try {
        logger.debug('_getCamDeviceId() | calling _updateCams()')

        await self._updateCams()

        const cams = Array.from(self.cams.values())
        return cams[0] ? cams[0].deviceId : null
      } catch (error) {
        logger.error('_getCamDeviceId() failed: %o', error)
      }
    },
    closeTransports() {
        var self = this
        if(self.sendTransport) {
            self.sendTransport.close()
            self.sendTransport = null
        }
        if(self.recvTransport) {
            self.recvTransport.close()
            self.recvTransport = null
        }
    }
  }
}
</script>

<style>
body {
  margin: 0;
  background-color: #313131;
  color: #fff;
}

#app {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}

#app > .el-container {
  margin-bottom: 40px;
}

.el-header,
.el-footer {
  line-height: 60px;
}

.demo-block {
  border: 1px solid #ebebeb;
  border-radius: 3px;
  transition: 0.2s;
  background-color: #ececec;
  padding-top: 16px;
  margin-bottom: 8px;
}

video {
  width: 360px;
  background-color: #000;
}

/* 水平镜像翻转 */
/*
video#localVideo {
    transform: rotateY(180deg);
}
*/
</style>
