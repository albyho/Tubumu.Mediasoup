<template>
  <div id="app">
    <el-container>
      <el-header>Demo - 服务模式: {{serveModeText}}</el-header>
        <el-container>
          <el-aside width="400px">            
            <div class="demo-block">
              <el-form ref="connectForm" :inline="true" label-width="80px" size="mini">
                <el-form-item label="Peer:">
                  <el-select v-model="connectForm.peerId" :disabled="connectForm.isConnected" clearable placeholder="请选择">
                    <el-option :label="`Peer ${index}${index == 8 || index === 9 ? '(admin)' : ''}`" v-for="(item, index) in accessTokens" :key="item" :value="index"></el-option>
                  </el-select>
                </el-form-item>
                <el-form-item>
                  <el-button type="primary" @click="handleConnect">{{(!connectForm.isConnected ? "Connect" : "Disconnect")}}</el-button>
                </el-form-item>
              </el-form>
            </div>
            <div class="demo-block" v-if="connectForm.isConnected">
              <el-form ref="roomForm" :inline="true" :model="roomForm" label-width="80px" size="mini">
                <el-form-item label="Room:">
                  <el-select v-model="roomForm.roomId" :disabled="roomForm.isJoinedRoom" clearable placeholder="请选择">
                    <el-option :label="`Room ${index}`" v-for="(item, index) in rooms" :key="item" :value="index"></el-option>
                  </el-select>
                </el-form-item>
                <el-form-item>
                  <el-button type="primary" @click="handleJoinRoom">{{(!roomForm.isJoinedRoom ? "Join" : "Leave")}}</el-button>
                </el-form-item>
              </el-form>
            </div>
            <div class="demo-block" v-if="connectForm.isConnected&&roomForm.isJoinedRoom">
              <el-form ref="peersForm" :model="peersForm" label-width="80px" size="mini">
                <el-form-item>
                  <el-table
                    ref="singleTable"
                    :data="peersForm.peers"
                    highlight-current-row
                    @current-change="onPeerNodeClick"
                    style="width: 100%">
                    <el-table-column
                      type="index"
                      width="50">
                    </el-table-column>
                    <el-table-column
                      property="displayName"
                      label="DisplayName">
                    </el-table-column>
                  </el-table>
               </el-form-item>
              </el-form>
            </div>
          </el-aside>
          <el-main>
            <video id="localVideo" ref="localVideo" v-if="!!camProducer" :srcObject.prop="localVideoStream" autoplay playsinline />
            <video v-for="(value, key) in remoteVideoStreams" :key="key" :srcObject.prop="value" autoplay playsinline />
            <audio v-for="(value, key) in remoteAudioStreams" :key="key" :srcObject.prop="value" autoplay />
          </el-main>
        </el-container>
    </el-container>
  </div>
</template>

<script>
import Logger from './lib/Logger';
import * as mediasoupClient from 'mediasoup-client';
import * as signalR from '@microsoft/signalr';

// eslint-disable-next-line no-unused-vars
const VIDEO_CONSTRAINS = {
  qvga: { width: { ideal: 320 }, height: { ideal: 240 } },
  vga: { width: { ideal: 640 }, height: { ideal: 480 } },
  '720P': { width: { ideal: 1280 }, height: { ideal: 720 } },
  '1080P': { width: { ideal: 1920 }, height: { ideal: 1080 } },
  '4k': { width: { ideal: 4096 }, height: { ideal: 2160 } }
};

const PC_PROPRIETARY_CONSTRAINTS = {
  optional: [{ googDscp: true }]
};

// eslint-disable-next-line no-unused-vars
const WEBCAM_SIMULCAST_ENCODINGS = [
  { scaleResolutionDownBy: 4, maxBitrate: 500000 },
  { scaleResolutionDownBy: 2, maxBitrate: 1000000 },
  { scaleResolutionDownBy: 1, maxBitrate: 5000000 }
];

// Used for VP9 cam video.
// eslint-disable-next-line no-unused-vars
const WEBCAM_KSVC_ENCODINGS = [
  { scalabilityMode: 'S3T3_KEY' }
];

// Used for simulcast screen sharing.
// eslint-disable-next-line no-unused-vars
const SCREEN_SHARING_SIMULCAST_ENCODINGS =
[
  { dtx: true, maxBitrate: 1500000 },
  { dtx: true, maxBitrate: 6000000 }
];

// Used for VP9 screen sharing.
// eslint-disable-next-line no-unused-vars
const SCREEN_SHARING_SVC_ENCODINGS =
[
  { scalabilityMode: 'S3T3', dtx: true }
];

const logger = new Logger('App');

// 'mediasoup-client:* tubumu-meeting-demo-client:*'
localStorage.setItem('debug', 'mediasoup-client:* tubumu-meeting-demo-client:*');

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
      cams: {},
      audioDevices: {},
      camProducer: null,
      micProducer: null,
      useSimulcast: false,
      forceH264: false,
      forceVP9: false,
      forceTcp: false,
      localAudioStream: null,
      localVideoStream: null,
      remoteVideoStreams: {},
      remoteAudioStreams: {},
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
        peers: [],
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
        useDataChannel: true
      },
      rooms: [
        "Room 0",
        "Room 1",
        "Room 2",
        "Room 3",
        "Room 4",
        "Room 5",
        "Room 6",
        "Room 7",
        "Room 8",
        "Room 9"
      ],
      accessTokens: [
          'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiMCIsIm5iZiI6MTU4NDM0OTA0NiwiZXhwIjoxNTg2OTQxMDQ2LCJpc3MiOiJpc3N1ZXIiLCJhdWQiOiJhdWRpZW5jZSJ9.lX6ff6ZFNb4z-4otDcUAZ48qwD8rZAGM5_Rt4HlHgug',
          'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiMSIsIm5iZiI6MTU4NDM0OTA0NiwiZXhwIjoxNTg2OTQxMDQ2LCJpc3MiOiJpc3N1ZXIiLCJhdWQiOiJhdWRpZW5jZSJ9.5w00ixg06pRPxcdbtbmRVI6Wy_Ta9qsSJc3D7PE3chQ',
          'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiMiIsIm5iZiI6MTU4NDM0OTA0NiwiZXhwIjoxNTg2OTQxMDQ2LCJpc3MiOiJpc3N1ZXIiLCJhdWQiOiJhdWRpZW5jZSJ9.q2tKWUa6i4u0VpZDhA8Fw92NoV_g9YQeWD-OeF7fAvU',
          'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiMyIsIm5iZiI6MTU4NDM0OTA0NiwiZXhwIjoxNTg2OTQxMDQ2LCJpc3MiOiJpc3N1ZXIiLCJhdWQiOiJhdWRpZW5jZSJ9.ekjd10Bortc1q34Ani1F_Gw9KQwS4qRFIU715pE1lGo',
          'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiNCIsIm5iZiI6MTU4NDM0OTA0NiwiZXhwIjoxNTg2OTQxMDQ2LCJpc3MiOiJpc3N1ZXIiLCJhdWQiOiJhdWRpZW5jZSJ9.6hZ4GtamurlemceCV7I5vT-UQkbszRxys5h8QOiOhcE',
          'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiNSIsIm5iZiI6MTU4NDM0OTA0NiwiZXhwIjoxNTg2OTQxMDQ2LCJpc3MiOiJpc3N1ZXIiLCJhdWQiOiJhdWRpZW5jZSJ9.nV4fr0tYGDR7zsykNoFYERdVSUPSqmhGdOkPqBjK1qw',
          'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiNiIsIm5iZiI6MTU4NDM0OTA0NiwiZXhwIjoxNTg2OTQxMDQ2LCJpc3MiOiJpc3N1ZXIiLCJhdWQiOiJhdWRpZW5jZSJ9.ftPY4GXCyyuaIaeihsexS99iluUCSZo6XSUOfdf_gGs',
          'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiNyIsIm5iZiI6MTU4NDM0OTA0NiwiZXhwIjoxNTg2OTQxMDQ2LCJpc3MiOiJpc3N1ZXIiLCJhdWQiOiJhdWRpZW5jZSJ9.V_S8YRRCOWP5-TG8S_cvlWdaXEKpsgY34lt3FiCgqGw',
          'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiOCIsIm5iZiI6MTU4NDM0OTA0NiwiZXhwIjoxNTg2OTQxMDQ2LCJpc3MiOiJpc3N1ZXIiLCJhdWQiOiJhdWRpZW5jZSJ9.fl1jA6DEQyXwIb6KRDUlEthR2G7-Je3OiaZHJ3tzk9M',
          'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiOSIsIm5iZiI6MTU4NDM0OTA0NiwiZXhwIjoxNTg2OTQxMDQ2LCJpc3MiOiJpc3N1ZXIiLCJhdWQiOiJhdWRpZW5jZSJ9.3Hnnkoxe52L7joy99dXkcIjHtz9FUitf4BGYCYjyKdE'
        ]
    };
  },
  computed: {
    isAdmin: function () {
      return this.connectForm.peerId === 8 || this.connectForm.peerId === 9
    },
    serveModeText: function () {
      return this.serveMode === 'Open' ? '开放(Open)模式' : this.serveMode === 'Invite' ? '邀请(Invite)模式' : '拉取(Pull)模式'
    }
  },
  async mounted() {
    var searchParams = new URLSearchParams(location.search.replace('?', ''));
    this.connectForm.peerId = searchParams.get('peerId') || searchParams.get('peerid');
    if(this.connectForm.peerId) {
      this.connectForm.peerId = parseInt(this.connectForm.peerId);
    }

    this.roomForm.roomId =  searchParams.get('roomId') || searchParams.get('roomid');
    if(this.roomForm.roomId) {
      this.roomForm.roomId = parseInt(this.roomForm.roomId);
    }

    // For Testing
    // this.form.produce = this.connectForm.peerId !== '0' && this.connectForm.peerId !== '1';
  },
  methods: {
    async handleConnect() {
      if(this.connectForm.isConnected) {
        if(this.connection) {
         await this.connection.stop();
        }
        this.connectForm.isConnected = false;
        this.roomForm.isJoinedRoom = false;
        this.camClosed();
        this.micClosed();
        this.peersForm.peers = [];
        this.remoteVideoStreams = {};
        this.remoteAudioStreams = {};
        return;
      }

      if(!this.connectForm.peerId && this.connectForm.peerId !== 0) {
        this.$message.error('Select a peer, please.');
        return;
      }

      try {
        const host = process.env.NODE_ENV === 'production' ? '' : `https://${window.location.hostname}:9001`;
        this.connection = new signalR.HubConnectionBuilder()
          .withUrl(
            `${host}/hubs/meetingHub`, {
              accessTokenFactory: () => this.accessTokens[this.connectForm.peerId],
              skipNegotiation: true,
              transport: signalR.HttpTransportType.WebSockets,
            }
          )
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
          .build();

        this.connection.onclose(e => {
          this.connectForm.isConnected = false;
          this.roomForm.isJoinedRoom = false;
          this.peersForm.peers = [];
          this.disableMic().catch(() => {});
          this.disableCam().catch(() => {});
          if(e) {
            logger.error(e)
          }
        });

        this.connection.on('Notify', async data => {
          await this.processNotification(data);
        });
        await this.connection.start();
        await this.start();
        this.connectForm.isConnected = true;
      } catch (e) {
        logger.debug(e.message);
      }
    },
    async start() {
      let result = await this.connection.invoke('GetServeMode');
      if (result.code !== 200) {
        logger.error(`start() | GetServeMode failed: ${result.message}`);
        this.$message.error(`start() | GetServeMode failed: ${result.message}`);
        return;
      }
      this.serveMode = result.data.serveMode;

      result = await this.connection.invoke('GetRouterRtpCapabilities');
      if (result.code !== 200) {
        logger.error(`start() | GetRouterRtpCapabilities failed: ${result.message}`);
        this.$message.error(`start() | GetRouterRtpCapabilities failed: ${result.message}`);
        return;
      }

      const routerRtpCapabilities = result.data;
      this.mediasoupDevice = new mediasoupClient.Device();
      await this.mediasoupDevice.load({
        routerRtpCapabilities
      });

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
      result = await this.connection.invoke('Join', {
        rtpCapabilities: this.mediasoupDevice.rtpCapabilities,
        sctpCapabilities: this.form.useDataChannel && this.form.consume
						? this.mediasoupDevice.sctpCapabilities
						: undefined,
        displayName: null,
        sources: ['audio:mic', 'video:cam'],
        appData: {}
      });
      if (result.code !== 200) {
        logger.error(`processNotification() | Join failed: ${result.message}`);
        this.$message.error(`processNotification() | Join failed: ${result.message}`);
        return;
      }
    },
    async handleJoinRoom() {
      if(this.roomForm.isJoinedRoom) {
        let result = await this.connection.invoke('LeaveRoom');
        if (result.code !== 200) {
          logger.error(`handleJoinRoom() | LeaveRoom failed: ${result.message}`)
          this.$message.error(`handleJoinRoom() | LeaveRoom failed: ${result.message}`);
          return;
        }
        this.peersForm.peers = [];
        this.roomForm.isJoinedRoom = false;
        await this.disableMic();
        await this.disableCam();
        return;
      } 
      if(!this.roomForm.roomId && this.roomForm.roomId !== 0) {
        this.$message.error('Select a room, please.');
        return;
      }

      let result = await this.connection.invoke('JoinRoom', {
        roomId: this.roomForm.roomId.toString(),
        role: this.isAdmin ? "admin" : "normal"
      });
      if (result.code !== 200) {
        logger.error('handleJoinRoom() | JoinRoom failed.');
        this.$message.error(`handleJoinRoom() | JoinRoom failed: ${result.message}`);
        return;
      }

      let {peers} = result.data;
      for(let i = 0; i < peers.length; i++) {
        this.peersForm.peers.push(peers[i]);
      }
      this.roomForm.isJoinedRoom = true;

      if(this.form.produce) {
        // Join成功，CreateSendWebRtcTransport(生产) 
        result = await this.connection.invoke('CreateSendWebRtcTransport', {
          forceTcp: this.forceTcp,
          sctpCapabilities: this.form.useDataChannel
							? this.mediasoupDevice.sctpCapabilities
							: undefined
        });
        if (result.code !== 200) {
          logger.error(`handleJoinRoom() | CreateSendWebRtcTransport failed: ${result.message}`);
          this.$message.error(`handleJoinRoom() | CreateSendWebRtcTransport failed: ${result.message}`);
          return;
        }

        // CreateSendWebRtcTransport(生产)成功, createSendTransport
        this.sendTransport = this.mediasoupDevice.createSendTransport({
          id: result.data.transportId,
          iceParameters: result.data.iceParameters,
          iceCandidates: result.data.iceCandidates,
          dtlsParameters: result.data.dtlsParameters,
          sctpParameters: result.data.sctpParameters,
          iceServers: [],
          proprietaryConstraints: PC_PROPRIETARY_CONSTRAINTS
        });

        this.sendTransport.on(
          'connect',
          ({ dtlsParameters }, callback, errback) => {
            logger.debug('sendTransport.on() connect dtls: %o', dtlsParameters);
            this.connection
              .invoke('ConnectWebRtcTransport', {
                transportId: this.sendTransport.id,
                dtlsParameters
              })
              .then(callback)
              .catch(errback);
          }
        );

        this.sendTransport.on(
          'produce',
          // appData 需要包含 source
          // eslint-disable-next-line no-unused-vars
          async ({ kind, rtpParameters, appData }, callback, errback) => {
            logger.debug('sendTransport.on() produce, appData: %o', appData);
            try {
              const result = await this.connection.invoke('Produce', {
                transportId: this.sendTransport.id,
                kind,
                rtpParameters,
                source: appData.source,
                appData
              });
              if (result.code !== 200) {
                logger.error(`sendTransport 'produce' callbak | failed: ${result.message}`);
                this.$message.error(`sendTransport 'produce' callbak | failed: ${result.message}`);
                errback(new Error(result.message));
                return;
              }
              this.producers.set(result.data.id, result.data);
              callback({ id: result.data.id });
            } catch (error) {
              errback(error);
            }
          }
        );

        this.sendTransport.on('producedata', async (
					{
						sctpStreamParameters,
						label,
						protocol,
						appData
					},
					callback,
					errback
				) =>
				{
					logger.debug('"producedata" event: [sctpStreamParameters:%o, appData:%o]', sctpStreamParameters, appData);

					try
					{
						// eslint-disable-next-line no-shadow
						const { id } = await this._protoo.request(
							'ProduceData',
							{
								transportId : this.sendTransport.id,
								sctpStreamParameters,
								label,
								protocol,
								appData
							});

						callback({ id });
					}
					catch (error)
					{
						errback(error);
					}
        });
        
        this.sendTransport.on('connectionstatechange', connectionState => {
          logger.debug(`sendTransport.on() connectionstatechange: ${connectionState}`);
          if (connectionState === 'connected')
					{
						this.enableDataProducer();
					}
        });
      }
      // createSendTransport 成功, CreateRecvWebRtcTransport(消费)
      result = await this.connection.invoke('CreateRecvWebRtcTransport', {
        forceTcp: this.forceTcp,
        sctpCapabilities: this.form.useDataChannel
							? this._mediasoupDevice.sctpCapabilities
							: undefined
      });

      // CreateRecvWebRtcTransport(消费)成功, createRecvTransport
      this.recvTransport = this.mediasoupDevice.createRecvTransport({
        id: result.data.transportId,
        iceParameters: result.data.iceParameters,
        iceCandidates: result.data.iceCandidates,
        dtlsParameters: result.data.dtlsParameters,
        sctpParameters: result.data.sctpParameters,
        iceServers: []
      });

      this.recvTransport.on(
        'connect',
        ({ dtlsParameters }, callback, errback) => {
          logger.debug('recvTransport.on() connect dtls: %o', dtlsParameters);
          this.connection
            .invoke('ConnectWebRtcTransport', {
              transportId: this.recvTransport.id,
              dtlsParameters
            })
            .then(callback)
            .catch(errback);
        }
      );

      this.recvTransport.on('connectionstatechange', connectionState => {
        logger.debug(`recvTransport.on() connectionstatechange: ${connectionState}`);
      });

      if(this.serveMode === 'Open' && this.form.produce) {
        if(this.form.produceAudio) {
          await this.enableMic();
        }
        if(this.form.produceVideo) {
          await this.enableCam();
        }
      }

      if (this.serveMode !== 'Pull') {
        let result = await this.connection.invoke('Ready');
        if (result.code !== 200) {
          logger.error(`Ready() | failed: ${result.message}`);
          this.$message.error(`Ready() | failed: ${result.message}`);
          return;
        }
      }
    },
    async onPeerNodeClick(peer) {
      if(!peer) return;
      logger.debug('onPeerNodeClick() | %o', peer);
      if(this.serveMode === 'Pull') {
        await this.pull(peer.peerId, peer.sources)
      } else if(this.serveMode === 'Invite') {
        if(this.isAdmin) {
          await this.invite(peer.peerId, peer.sources)
        } 
        else
        {
          this.$message.error('仅管理员可进行邀请操作。');
        }
      }
    },
    async processNewConsumer(data) {
      const {
        producerPeerId,
        producerId,
        consumerId,
        kind,
        rtpParameters,
        //type, // mediasoup-client 的 Transport.ts 不使用该参数
        producerAppData,
        //producerPaused // mediasoup-client 的 Transport.ts 不使用该参数
      } = data;

      const consumer = await this.recvTransport.consume({
        id: consumerId,
        producerId,
        kind,
        rtpParameters,
        // producerAppData 中的 peerId 为生产者的 PeerId。
        appData: { ...producerAppData, producerPeerId } // Trick.
      });
      logger.debug('processNewConsumer() Consumer: %o', consumer);

      // Store in the map.
      this.consumers.set(consumer.id, consumer);

      consumer.on('transportclose', () => {
        this.consumers.delete(consumer.id);
      });

      const {
        // eslint-disable-next-line no-unused-vars
        spatialLayers,
        // eslint-disable-next-line no-unused-vars
        temporalLayers
      } = mediasoupClient.parseScalabilityMode(
        consumer.rtpParameters.encodings[0].scalabilityMode
      );

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

      const stream = new MediaStream();
      stream.addTrack(consumer.track);

      this.$set(kind === 'video' ? this.remoteVideoStreams : this.remoteAudioStreams, consumerId, stream);

      // We are ready. Answer the request so the server will
      // resume this Consumer (which was paused for now).
      logger.debug('processNewConsumer() ResumeConsumer');
      const result = await this.connection.invoke('ResumeConsumer', consumerId);
      if (result.code !== 200) {
        logger.error(`processNewConsumer() | ResumeConsumer failed: ${result.message}`);
        this.$message.error(`processNewConsumer() | ResumeConsumer failed: ${result.message}`);
        return;
      }
    },
    async processNewDataConsumer(data) {
      const {
        dataProducerPeerId, // NOTE: Null if bot.
        dataProducerId,
        dataCosumerId,
        sctpStreamParameters,
        label,
        protocol,
        dataProducerAppData
      } = data;

      try
      {
        const dataConsumer = await this.recvTransport.consumeData(
          {
            dataCosumerId,
            dataProducerId,
            sctpStreamParameters,
            label,
            protocol,
            appData : { ...dataProducerAppData, dataProducerPeerId } // Trick.
          });

        // Store in the map.
        this.dataConsumers.set(dataConsumer.id, dataConsumer);

        dataConsumer.on('transportclose', () =>
        {
          this.dataConsumers.delete(dataConsumer.id);
        });

        dataConsumer.on('open', () =>
        {
          logger.debug('DataConsumer "open" event');
        });

        dataConsumer.on('close', () =>
        {
          logger.warn('DataConsumer "close" event');
          this.dataConsumers.delete(dataConsumer.id);
        });

        dataConsumer.on('error', (error) =>
        {
          logger.error('DataConsumer "error" event:%o', error);
        });

        dataConsumer.on('message', (message) =>
        {
          logger.debug('DataConsumer "message" event [streamId:%d]', dataConsumer.sctpStreamParameters.streamId);

          if (message instanceof ArrayBuffer)
          {
            const view = new DataView(message);
            const number = view.getUint32();

            if (number == Math.pow(2, 32) - 1)
            {
              logger.warn('dataChannelTest finished!');
              this.nextDataChannelTestNumber = 0;

              return;
            }

            if (number > this.nextDataChannelTestNumber)
            {
              logger.warn(
                'dataChannelTest: %s packets missing',
                number - this.nextDataChannelTestNumber);
            }

            this.nextDataChannelTestNumber = number + 1;

            return;
          }
          else if (typeof message !== 'string')
          {
            logger.warn('ignoring DataConsumer "message" (not a string)');

            return;
          }

          logger.debug(`New message: ${message}`);
        });
      }
      catch (error)
      {
        logger.error('"newDataConsumer" request failed:%o', error);
        throw error;
      }
    },
    async pull(peerId, sources) {
      const result = await this.connection.invoke('Pull', {
        peerId,
        sources
      });
      if (result.code !== 200) {
        logger.error(`pull() | failed: ${result.message}`);
        this.$message.error(`pull() | failed: ${result.message}`);
        return;
      }
    },
    async invite(peerId, sources) {
      const result = await this.connection.invoke('Invite', {
        peerId,
        sources
      });
      if (result.code !== 200) {
        logger.error(`invite() | failed: ${result.message}`);
        this.$message.error(`invite() | failed: ${result.message}`);
        return;
      }
    },
    async processNotification(data) {
      logger.debug('processNotification() | %o', data);
      switch (data.type) {
        case 'newConsumer': {
            await this.processNewConsumer(data.data);
            
            break;
        }

        case 'newDataConsumer': {
            await this.processNewDataConsumer(data.data);

            break;
        }

        case 'producerScore': {
          // eslint-disable-next-line no-unused-vars
          const { producerId, score } = data.data;

          break;
        }

        case 'peerJoinRoom': {
          // eslint-disable-next-line no-unused-vars
          const {peer} = data.data;
          this.peersForm.peers.push(peer);
          break;
        }

        case 'peerLeaveRoom':
        {
          // eslint-disable-next-line no-unused-vars
          const { peerId } = data.data;
          let idx = -1;
          for(let i = 0; i < this.peersForm.peers.length; i++) {
            if(this.peersForm.peers[i].peerId === peerId) {
              idx = i;
              break;
            }
          }
          if(idx >= 0){
            this.peersForm.peers.splice(idx, 1);
          }
          break;
        }
        
        case 'peerRoomAppDataChanged': {

          break;
        }

        case 'produceSources':
        {
          if(!this.form.produce) break;

          const { /*roomId, */sources } = data.data;
          for(let i =0; i < sources.length; i++){
            if(sources[i] === 'audio:mic' && this.mediasoupDevice.canProduce('audio')) {
              if(!this.micProducer) {
                await this.enableMic();
              }
            } else if(sources[i] === 'video:cam' && this.mediasoupDevice.canProduce('video')) {
              if(!this.camProducer) {
                await this.enableCam();
              }
            }
          }

          break;
        }

        case 'downlinkBwe':
        {
          logger.debug('\'downlinkBwe\' event: %o', data.data);

          break;
        }

        case 'consumerClosed': {
          const { consumerId } = data.data;
          const consumer = this.consumers.get(consumerId);

          if (!consumer) break;

          this.$delete(consumer.kind === 'video' ? this.remoteVideoStreams : this.remoteAudioStreams, consumerId)
          consumer.close();
          this.consumers.delete(consumerId);

          break;
        }

        case 'consumerPaused': {
          const { consumerId } = data.data;
          const consumer = this.consumers.get(consumerId);

          if (!consumer) break;

          break;
        }

        case 'consumerResumed': {
          const { consumerId } = data.data;
          const consumer = this.consumers.get(consumerId);

          if (!consumer) break;

          break;
        }

        case 'consumerLayersChanged': {
          // eslint-disable-next-line no-unused-vars
          const { consumerId, spatialLayer, temporalLayer } = data.data;
          const consumer = this.consumers.get(consumerId);

          if (!consumer) break;

          break;
        }

        case 'consumerScore': {
          const { consumerId } = data.data;
          const consumer = this.consumers.get(consumerId);

          if (!consumer) break;

          break;
        }

        case 'producerClosed': {
          const { producerId } = data.data;
          const producer = this.producers.get(producerId);

          if (!producer) break;

          if(producer.source === 'video:cam') {
            this.camClosed();
          } else if(producer.source === 'audio:mic') {
            this.micClosed();
          }

          break;
        }

        case 'peerLeave': {
          const { peerId } = data.data;
          for(let i = this.peersForm.peers.length - 1; i > 0; i--) {
            if(this.peersForm.peers[i].peerId === peerId) {
              this.peersForm.peers.splice(i, 1);
              break;
            }
          }
          break;
        }

        case 'producerVideoOrientationChanged': {
          break;
        }

        case 'activeSpeaker': {
          break;
        }

        default: {
          logger.error('unknown data.type, data:%o', data);
        }
      }
    },
    async enableDataProducer()
    {
      logger.debug('enableChatDataProducer()');

      if (!this.form.useDataChannel)
        return;

      try
      {
        this.dataProducer = await this.sendTransport.produceData(
          {
            ordered        : false,
            maxRetransmits : 1,
            label          : 'chat',
            priority       : 'medium',
            appData        : { info: '' }
          });

        this.dataProducer.on('transportclose', () =>
        {
          this.dataProducer = null;
        });

        this.dataProducer.on('open', () =>
        {
          logger.debug('DataProducer "open" event');
        });

        this.dataProducer.on('close', () =>
        {
          logger.error('DataProducer "close" event');

          this.dataProducer = null;
        });

        this.dataProducer.on('error', (error) =>
        {
          logger.error('chat DataProducer "error" event:%o', error);
        });

        this.dataProducer.on('bufferedamountlow', () =>
        {
          logger.debug('chat DataProducer "bufferedamountlow" event');
        });
      }
      catch (error)
      {
        logger.error('enableDataProducer() | failed:%o', error);

        throw error;
      }
    },
    async enableMic() {
      logger.debug('enableMic()');

      if (this.micProducer) return;
      if (this.mediasoupDevice && !this.mediasoupDevice.canProduce('audio')) {
        logger.error('enableMic() | cannot produce audio');
        return;
      }

      let track;

      try {
        const deviceId = await this._getAudioDeviceId();

        const device = this.audioDevices[deviceId];

        if (!device) throw new Error('no audio devices');

        logger.debug(
          'enableMic() | new selected audio device [device:%o]',
          device
        );

        logger.debug('enableMic() | calling getUserMedia()');

        const stream = await navigator.mediaDevices.getUserMedia({
          audio: {
            deviceId: { ideal: deviceId }
          }
        });
        this.localAudioStream = stream;

        track = stream.getAudioTracks()[0];

        this.micProducer = await this.sendTransport.produce({
          track,
          codecOptions: {
            opusStereo: 1,
            opusDtx: 1
          },
          appData: { source: 'audio:mic' }
        });

        this.micProducer.on('transportclose', () => {
          this.micProducer = null;
        });

        this.micProducer.on('trackended', () => {
          this.disableMic().catch(() => {});
        });

      } catch (error) {
        console.log('enableMic() failed: %o', error);
        logger.error('enableMic() failed: %o', error);
        if (track) track.stop();
      }
    },
    async disableMic() {
      logger.debug('disableMic()');
      if (!this.micProducer) return;

      const micProducerId = this.micProducer.id;
      this.micClosed();

      try {
        await this.connection.invoke('CloseProducer', micProducerId);
      } catch (error) {
        logger.error('disableMic() [error:"%o"]', error);
      }

    },
    micClosed() {
      if(this.micProducer)
      {
        this.micProducer.close();
        this.micProducer = null;
      }
      this.localAudioStream = null;
    },
    async enableCam() {
      logger.debug('enableCam()');

      if (this.camProducer) return;
      if (this.mediasoupDevice && !this.mediasoupDevice.canProduce('video')) {
        logger.error('enableCam() | cannot produce video');
        return;
      }

      let track;

      try {
        const deviceId = await this._getCamDeviceId();

        logger.debug(`enableCam() | cam: ${deviceId}`);

        const device = this.cams.get(deviceId);

        if (!device) throw new Error(`no cam devices: ${JSON.stringify(this.cams)}`);

        logger.debug('enableCam() | new selected cam [device:%o]', device);

        logger.debug('enableCam() | calling getUserMedia()');

        //const stream = await navigator.mediaDevices.getUserMedia({ video: true })
        //*
        const stream = await navigator.mediaDevices.getUserMedia({
          video: {
            deviceId: { ideal: deviceId },
            ...VIDEO_CONSTRAINS['720P']
          }
        });
        //*/
        this.localVideoStream = stream;

        track = stream.getVideoTracks()[0];

        let encodings;
        let codec;
        const codecOptions =
        {
          videoGoogleStartBitrate : 1000
        };
      
        if (this.forceH264)
        {
          codec = this.mediasoupDevice.rtpCapabilities.codecs
            .find((c) => c.mimeType.toLowerCase() === 'video/h264');

          if (!codec)
          {
            throw new Error('desired H264 codec+configuration is not supported');
          }
        }
        else if (this.forceVP9)
        {
          codec = this.mediasoupDevice.rtpCapabilities.codecs
            .find((c) => c.mimeType.toLowerCase() === 'video/vp9');

          if (!codec)
          {
            throw new Error('desired VP9 codec+configuration is not supported');
          }
        }
      
        if (this.useSimulcast) {
          // If VP9 is the only available video codec then use SVC.
          const firstVideoCodec = this.mediasoupDevice.rtpCapabilities.codecs.find(
            c => c.kind === 'video'
          );

          if (firstVideoCodec.mimeType.toLowerCase() === 'video/vp9')
            encodings = WEBCAM_KSVC_ENCODINGS;
          else encodings = WEBCAM_SIMULCAST_ENCODINGS;
        }

        this.camProducer = await this.sendTransport.produce({
          track,
          encodings,
          codecOptions,
          codec,
          appData: { source: 'video:cam' }
        });

        this.camProducer.on('transportclose', () => {
          this.camProducer = null;
        });

        this.camProducer.on('trackended', () => {
          this.disableCam().catch(() => {});
        });
        logger.debug('enableCam() succeeded');
      } catch (error) {
        logger.error('enableCam() failed:%o', error);

        if (track) track.stop();
      }
    },
    async disableCam() {
      logger.debug('disableCam()');

      if (!this.camProducer) return;

      const camProducerId = this.camProducer.id;
      this.camClosed();

      try {
        await this.connection.invoke('CloseProducer', camProducerId);
      } catch (error) {
        logger.error('disableCam() [error:"%o"]', error);
      }
    },
    camClosed() {
      if(this.camProducer)
      {
        this.camProducer.close();
        this.camProducer = null;
      }
      this.localVideoStream = null;
    },
    async _updateAudioDevices() {
      logger.debug('_updateAudioDevices()');

      // Reset the list.
      this.audioDevices = {};

      try {
        logger.debug('_updateAudioDevices() | calling enumerateDevices()');

        const devices = await navigator.mediaDevices.enumerateDevices();

        for (const device of devices) {
          if (device.kind !== 'audioinput') continue;

          this.audioDevices[device.deviceId] = device;
        }
      } catch (error) {
        logger.error('_updateAudioDevices() failed: %o', error);
      }
    },
    async _updateCams() {
      logger.debug('_updateCams()');

      // Reset the list.
      this.cams = new Map();

      try {
        logger.debug('_updateCams() | calling enumerateDevices()');

        const devices = await navigator.mediaDevices.enumerateDevices();

        logger.debug('_updateCams() | %o', devices);
        for (const device of devices) {
          if (device.kind !== 'videoinput') continue;
          logger.debug('_updateCams() | %o', device);
          this.cams.set(device.deviceId, device);
        }
      } catch (error) {
        logger.error('_updateCams() failed: %o', error);
      }
    },
    async _getAudioDeviceId() {
      logger.debug('_getAudioDeviceId()');

      try {
        logger.debug('_getAudioDeviceId() | calling _updateAudioDeviceId()');

        await this._updateAudioDevices();

        const audioDevices = Object.values(this.audioDevices);
        return audioDevices[0] ? audioDevices[0].deviceId : null;
      } catch (error) {
        logger.error('_getAudioDeviceId() failed: %o', error);
      }
    },
    async _getCamDeviceId() {
      logger.debug('_getCamDeviceId()');

      try {
        logger.debug('_getCamDeviceId() | calling _updateCams()');

        await this._updateCams();

        const cams = Array.from(this.cams.values());
        return cams[0] ? cams[0].deviceId : null;
      } catch (error) {
        logger.error('_getCamDeviceId() failed: %o', error);
      }
    }
  }
};
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

.demo-block
{
  border:1px solid #ebebeb;
  border-radius:3px;
  transition:.2s;
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
