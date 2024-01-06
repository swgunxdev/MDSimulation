//
// File Name: Constants.cs
// ----------------------------------------------------------------------
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using MDModeling.Providers;

namespace MDModeling
{
		public class PayloadHeader : IToFromByteArray
		{
			public UInt16 PayloadType; // The type of the payload
			public UInt32 PayloadLength; // The length of the payload
			public UInt16 PayloadVersion; // The version of the payload

            public PayloadHeader()
            {
                PayloadLength = 0;
                PayloadType = 0;
                PayloadVersion = 0;
            }

            public PayloadHeader(UInt32 pLength, UInt16 pType, UInt16 pVer)
                : this()
            {
                PayloadLength = pLength;
                PayloadType = pType;
                PayloadVersion = pVer;
            }

            public static UInt32 SizeOfHeader()
            {
                return (UInt32)(Marshaller.SizeOf<UInt16>() + Marshaller.SizeOf<UInt32>() + Marshaller.SizeOf<UInt16>());
            }

            public byte[] ToByteArray()
            {
                MemoryStream ms = new MemoryStream();

                byte[] pType = BytesProvider<UInt16>.Default.GetBytes(PayloadType);
                ms.Write(pType, 0, pType.Length);
                byte[] plength = BytesProvider<UInt32>.Default.GetBytes(PayloadLength);
                ms.Write(plength, 0, plength.Length);
                byte[] pVer = BytesProvider<UInt16>.Default.GetBytes(PayloadVersion);
                ms.Write(pVer, 0, pVer.Length);
                return ms.ToArray();
            }

            public int FromByteArray(byte[] data, int offset)
            {
                int curPos = offset;

                PayloadType = ByteArryTypeProvider<UInt16>.Default.Convert(data, curPos);
                curPos += Marshaller.SizeOf<UInt16>();

                PayloadLength = ByteArryTypeProvider<UInt32>.Default.Convert(data, curPos);
                curPos += Marshaller.SizeOf<UInt32>();
                PayloadVersion = ByteArryTypeProvider<UInt16>.Default.Convert(data, curPos);
                curPos += Marshaller.SizeOf<UInt16>();
                return curPos;
            }
        }

		public class Payload : IToFromByteArray
		{
			public PayloadHeader Header;
			public byte[] PayloadData; // The data payload of the container

            public Payload()
            {
                Header = new PayloadHeader(0, 0, 0);
                PayloadData = new byte[0];
            }

            public Payload(UInt16 payloadType, UInt16 payloadVersion)
                : this()
            {
                Header = new PayloadHeader(0, payloadType, payloadVersion);
            }

            public Payload(UInt16 payloadType, UInt16 payloadVersion, byte [] payload)
                : this(payloadType, payloadVersion)
            {
                PayloadData = new byte[payload.Length];
                Header.PayloadLength = (UInt32)payload.Length;
                payload.CopyTo(PayloadData, 0);
            }

            public void SetPayload(byte [] payload)
            {
                PayloadData = new byte [payload.Length];
                payload.CopyTo(PayloadData,0);
                Header.PayloadLength = (UInt32)payload.Length;
            }

            public UInt32 SizeOfPayload { get { return PayloadHeader.SizeOfHeader() + (UInt32)PayloadData.Length; } }

            public byte[] ToByteArray()
            {
                MemoryStream ms = new MemoryStream();

                byte[] header = Header.ToByteArray();
                ms.Write(header, 0, header.Length);
                ms.Write(PayloadData, 0, PayloadData.Length);
                return ms.ToArray();
            }

            public int FromByteArray(byte[] data, int offset)
            {
                int curPos = offset;

                Header = new PayloadHeader();
                curPos = Header.FromByteArray(data, curPos);
                PayloadData = new byte[Header.PayloadLength];
                Buffer.BlockCopy(data, curPos, PayloadData, 0, (int)Header.PayloadLength);
                curPos += (int)Header.PayloadLength;
                return curPos;
            }
        }

        public class PacketHeader : IToFromByteArray
        {
            public const UInt64 PacketSig = 0xDEADBEEFDEADBEEF;
            public const UInt16 PacketVer = 0x0001;

            public UInt64 PacketSignature; // The constant signature for all containers
            public UInt32 PacketLength; // The length of the container and the payload
            public UInt16 PacketVersion; // The version number of the container
            public UInt64 Reserved1; // Reserved for future use
            public UInt64 Reserved2; // Reserved for future use

            public PacketHeader()
            {
                PacketSignature = PacketSig;
                PacketLength = 0;
                PacketVersion = PacketVer;
                Reserved1 = 0L;
                Reserved2 = 0L;
            }

            public PacketHeader(UInt16 pVer)
                : this()
            {
                PacketVersion = pVer;
            }

            public static UInt32 SizeOfHeader()
            {
                return (UInt32)(Marshaller.SizeOf<UInt64>() * 3 + Marshaller.SizeOf<UInt32>()
                                                             + Marshaller.SizeOf<UInt16>());
            }

            public byte[] ToByteArray()
            {
                MemoryStream ms = new MemoryStream();

                byte[] signature = BytesProvider<UInt64>.Default.GetBytes(PacketSignature);
                ms.Write(signature, 0, signature.Length);
                byte[] plength = BytesProvider<UInt32>.Default.GetBytes(PacketLength);
                ms.Write(plength, 0, plength.Length);
                byte[] version = BytesProvider<UInt16>.Default.GetBytes(PacketVersion);
                ms.Write(version, 0, version.Length);
                byte[] reserve1 = BytesProvider<UInt64>.Default.GetBytes(PacketSignature);
                ms.Write(reserve1, 0, reserve1.Length);
                byte[] reserve2 = BytesProvider<UInt64>.Default.GetBytes(PacketSignature);
                ms.Write(reserve2, 0, reserve2.Length);
                return ms.ToArray();
            }

            public int FromByteArray(byte[] data, int offset)
            {
                int curPos = offset;

                PacketSignature = ByteArryTypeProvider<UInt64>.Default.Convert(data, curPos);
                if (PacketHeader.PacketSig != PacketSignature) return 0;
                curPos += Marshaller.SizeOf<UInt64>();

                PacketLength = ByteArryTypeProvider<UInt32>.Default.Convert(data, curPos);
                curPos += Marshaller.SizeOf<UInt32>();
                PacketVersion = ByteArryTypeProvider<UInt16>.Default.Convert(data, curPos);
                curPos += Marshaller.SizeOf<UInt16>();

                // skip over the reserved fields
                curPos += Marshaller.SizeOf<UInt64>();
                curPos += Marshaller.SizeOf<UInt64>();
                return curPos;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Packet Header --------------");
                sb.AppendFormat("Packet Signature: {0}", PacketSignature);
                sb.AppendFormat("Packet Length: {0}", PacketLength);
                sb.AppendFormat("Packet Version {0}", PacketVer);
                return sb.ToString();
            }


            public bool IsValid
            {
                get
                {
                    if (PacketHeader.PacketSig == PacketSignature &&
                      PacketLength > 0 && PacketVersion > 0)
                    {
                        return true;
                    }
                    return false;
                }
            }
        }

        public class Packet : IToFromByteArray
		{
			public PacketHeader Header;
			public Payload Payload; // The data payload of the container

            public Packet()
            {
                Header = new PacketHeader();
                Payload = new Payload();
            }

            public Packet(UInt16 pkVer, Payload payload)
                : this()
            {
                Header = new PacketHeader(pkVer);
                Header.PacketLength = PacketHeader.SizeOfHeader() + payload.SizeOfPayload;
                Payload = payload;
            }

            public Packet(byte [] data)
            {
                if(data.Length > 0) FromByteArray(data,0);
            }

            public byte[] ToByteArray()
            {
                MemoryStream ms = new MemoryStream();
                byte[] header = Header.ToByteArray();
                ms.Write(header, 0, header.Length);
                byte[] payload = Payload.ToByteArray();
                ms.Write(payload, 0, payload.Length);
                return ms.ToArray();
            }

            public int FromByteArray(byte[] data, int offset)
            {
                int curPos = offset;
                Header = new PacketHeader();
                curPos = Header.FromByteArray(data, curPos);
                if (curPos == 0) return offset; 

                this.Payload = new Payload();
                curPos = this.Payload.FromByteArray(data, curPos);
                return curPos;
            }


            public bool IsValid 
            { 
                get
                {
                    return Header.IsValid;
                }
            }
        }


    public class Constants
	{
        //public static Payload ByteArrayToPayload(byte[] byteArray, ref UInt32 index)
        //{
        //    Payload payload = new Payload();

        //    payload.Header.PayloadType = BitConverter.ToUInt16(byteArray, (int)index);
        //    index += (UInt32)Marshaller.SizeOf(payload.Header.PayloadType);
        //    payload.Header.PayloadLength = BitConverter.ToUInt32(byteArray, (int)index);
        //    index += (UInt32)Marshaller.SizeOf(payload.Header.PayloadLength);
        //    payload.Header.PayloadVersion = BitConverter.ToUInt16(byteArray, (int)index);
        //    index += (UInt32)Marshaller.SizeOf(payload.Header.PayloadVersion);
        //    UInt32 payloadLength = (UInt32)(payload.Header.PayloadLength - Marshaller.SizeOf(payload.Header));
        //    payload.PayloadData = new byte[payloadLength];
        //    for (int i = 0; i < payloadLength; i++)
        //    {
        //        payload.PayloadData[i] = byteArray[index++];
        //    }

        //    return payload;
        //}

        //public static Packet ByteArrayToPacket(byte[] byteArray, ref UInt32 index)
        //{
        //    Packet packet = new Packet();

        //    // verify the first 64 bits is the signature
        //    packet.Header.PacketSignature = BitConverter.ToUInt64(byteArray, (int)index);

        //    if (packet.Header.PacketSignature == PacketSig)
        //    {
        //        // get the rest of the packet
        //        index += sizeof(UInt64);
        //        packet.Header.PacketLength = BitConverter.ToUInt32(byteArray, (intBareMetal.SizeOf)index);
        //        index += sizeof(UInt32);
        //        packet.Header.PacketType = BitConverter.ToUInt16(byteArray, (int)index);
        //        index += sizeof(UInt16);

        //        packet.Header.PacketVersion = BitConverter.ToUInt16(byteArray, (int)index);
        //        index += sizeof(UInt16);
        //        packet.Header.Reserved1 = BitConverter.ToUInt64(byteArray, (int)index);
        //        index += sizeof(UInt64);
        //        packet.Header.Reserved2 = BitConverter.ToUInt64(byteArray, (int)index);
        //        index += sizeof(UInt64);
        //        packet.Payload = ByteArrayToPayload(byteArray, ref index);
        //    }

        //    return packet;
        //}

        //public static byte[] PayloadToByteArray(Payload payload)
        //{
        //    byte[] payloadArray = new byte[payload.Header.PayloadLength];
        //    UInt32 index = 0;

        //    BitConverter.GetBytes(payload.Header.PayloadType).CopyTo(payloadArray, index);
        //    index += (UInt32)Marshaller.SizeOf(payload.Header.PayloadType);
        //    BitConverter.GetBytes(payload.Header.PayloadLength).CopyTo(payloadArray, index);
        //    index += (UInt32)Marshaller.SizeOf(payload.Header.PayloadLength);
        //    BitConverter.GetBytes(payload.Header.PayloadVersion).CopyTo(payloadArray, index);
        //    index += (UInt32)Marshaller.SizeOf(payload.Header.PayloadVersion);
        //    payload.PayloadData.CopyTo(payloadArray, index);
        //    index += (UInt32)payload.PayloadData.Length;

        //    return payloadArray;
        //}

        //public static byte[] PacketToByteArray(Packet packet)
        //{
        //    byte[] packetArray = new byte[packet.Header.PacketLength];
        //    UInt32 index = 0;

        //    BitConverter.GetBytes(packet.Header.PacketSignature).CopyTo(packetArray, index);
        //    index += (UInt32)Marshaller.SizeOf(packet.Header.PacketSignature);
        //    BitConverter.GetBytes(packet.Header.PacketLength).CopyTo(packetArray, index);
        //    index += (UInt32)Marshaller.SizeOf(packet.Header.PacketLength);
        //    BitConverter.GetBytes(packet.Header.PacketType).CopyTo(packetArray, index);
        //    index += (UInt32)Marshaller.SizeOf(packet.Header.PacketType);
        //    BitConverter.GetBytes(packet.Header.PacketVersion).CopyTo(packetArray, index);
        //    index += (UInt32)Marshaller.SizeOf(packet.Header.PacketVersion);
        //    BitConverter.GetBytes(packet.Header.Reserved1).CopyTo(packetArray, index);
        //    index += (UInt32)Marshaller.SizeOf(packet.Header.Reserved1);
        //    BitConverter.GetBytes(packet.Header.Reserved2).CopyTo(packetArray, index);
        //    index += (UInt32)Marshaller.SizeOf(packet.Header.Reserved2);
        //    BitConverter.GetBytes(packet.Payload.Header.PayloadType).CopyTo(packetArray, index);
        //    index += (UInt32)Marshaller.SizeOf(packet.Payload.Header.PayloadType);
        //    BitConverter.GetBytes(packet.Payload.Header.PayloadLength).CopyTo(packetArray, index);
        //    index += (UInt32)Marshaller.SizeOf(packet.Payload.Header.PayloadLength);
        //    BitConverter.GetBytes(packet.Payload.Header.PayloadVersion).CopyTo(packetArray, index);
        //    index += (UInt32)Marshaller.SizeOf(packet.Payload.Header.PayloadVersion);
        //    packet.Payload.PayloadData.CopyTo(packetArray, index);
        //    index += (UInt32)packet.Payload.PayloadData.Length;

        //    return packetArray;
        //}


        //public static Payload CreateStringPayload(string str)
        //{
        //    Constants.Payload payload = new Payload();
        //    payload.Header.PayloadType = (UInt16)EDataType.UnicodeString;
        //    payload.Header.PayloadVersion = 1;
        //    payload.PayloadData = Encoding.Unicode.GetBytes(str);
        //    payload.Header.PayloadLength = (UInt32)(Marshaller.SizeOf(payload.Header) + payload.PayloadData.Length);

        //    return payload;
        //}

		public enum EDataType
		{
			UnicodeString = 0,
			UInt_8 = 1,
			Int_16 = 2,
			Int_32 = 3,
			Float = 4,
			Int_64 = 5,
			UInt_16 = 6,
			UInt_32 = 7,
			UInt_64 = 8,
			BinaryBlob = 9,
			Boolean = 10,
			Channel = 11,
			Input = 12,
			AutomaticGainControl = 13,
			Microphone = 14,
			Output = 15,
			TelcoReceive = 16,
			TelcoTransmit = 17,
			VoipTransmit = 18,
			UsbTransmit = 19,
			Filter = 20,
			Compressor = 21,
			Submix = 22,
			Fader = 23,
			EthernetValues = 24,
			UsbReceive = 25,
			VoipReceive = 26,
			VoipCommon = 27,
			VoipProxy = 28,
			VoipDialPlan = 29,
			SignalGenerator = 30,
			AdaptiveVolume = 31,
			FeedbackElimination = 32,
			NoiseGate = 33,
			SoundMask = 34,
			Delay = 35,
			GraphicEqualizer = 36,
			PowerAmp = 37,
			Limiter = 38,
			BeamFormingMicrophone = 39,
			Version = 40,
			TimedEvent = 41,
			Device = 42,
			GlobalSystem = 43,
			SNMPSettings = 44,
			Gate = 45,
			NoiseCancellation = 46,
			VoipTLS = 47,
			VoipOutboundProxy = 48,
			PowerAmpCommon = 49,
			GLink = 50,
			GatingGroup = 51,
			VirtualReference = 52,
			GPIO = 53,
			MatrixCrossPoint = 54,
			Matrix = 55,
			Command = 56,
			AVB = 57,
			ClassDOutput = 58,
			Ducker = 59,
			CobraNet = 60,
			AVBStream = 61,
			VTable = 62,
			VTableDevice = 63,
			PhoneBookEntry = 64,
			PhoneBook = 65,
			LocationID = 66,
			IDNode = 67,
			Connection = 68,
			TimedEvents = 69,
			Meter = 70,
			Timpanogos8x8 = 71,
			TimpanogosVoip = 72,
			Timpanogos8x8Amp = 73,
			Timpanogos8x8Plus = 74,
			TimpanogosEmpty = 75,
			AudioConfiguration = 76,
			Subroom = 77,
			PhysicalComponent = 78,
			Divider = 79,
			PhysicalRoom = 80,
			CrossPoint = 81,
			LogicalComponent = 82,
			ComponentGroup = 83,
		}

		public enum ERouteType
		{
			Data = 0,
			Discover = 1,
			Command = 2,
			Meter = 3,
			Connect = 4,
			Firmware = 5,
		}

		public enum EPayloadType
		{
			DiscoveryQuery = 0,
			DiscoveryResponse = 1,
			Disconnect = 2,
			ConnectQuery = 3,
			ConnectResponse = 4,
			FirmwareUpload = 5,
			FirmwareStatus = 6,
			SystemReset = 7,
			MeterQuery = 8,
			MeterResponse = 9,
			GetValueQuery = 10,
			GetValueResponse = 11,
			SetValue = 12,
			SetValueResponse = 13,
			GetLogQuery = 14,
			GetLogResponse = 15,
		}

        public enum EProperties
        {
            Name = 0,
            OnOffToggle = 1,
            Gain = 2,
            Port = 3,
            PhoneNumber = 4,
            Threshold = 5,
            Target = 6,
            Attack = 7,
            GateRatio = 8,
            GateMode = 9,
            GateHold = 10,
            OffAttenuation = 11,
            DecayRate = 12,
            MixerSelection = 13,
            AmbientLevel = 14,
            ReferenceSelectionChannel = 15,
            ReferenceSelectionObject = 16,
            NonLinearProcessing = 17,
            NoiseCancellationLevel = 18,
            TelcoAdaptMode = 19,
            AutoAnswerRings = 20,
            RingerSelection = 21,
            AutoDisconnect = 22,
            HookFlashDuration = 23,
            LastSpeedDial = 24,
            FQDN = 25,
            RingCadence = 26,
            RingOnTime = 27,
            RingOffTime = 28,
            Frequency = 29,
            GainSlope = 30,
            Bandwidth = 31,
            FilterType = 32,
            CompressorRatio = 33,
            Release = 34,
            DelayTime = 35,
            IPAddress = 36,
            SipNoAnswerTimeout = 37,
            SipRetransTimer = 38,
            RTPRTCPLog = 39,
            SipRetranTimer4 = 40,
            SipSRTPEnable = 41,
            SipSRTPCipher = 42,
            SipSRTPMac = 43,
            SipSRTPKdr = 44,
            SipSRTCPEnable = 45,
            DTMFRelayEnable = 46,
            DTMFPayload = 47,
            CodecPriority = 48,
            VADThreshold = 49,
            ProxyType = 50,
            VADNoiseMatching = 51,
            SipSessionTimer = 52,
            SipMinSETimer = 53,
            SipRegistrationTimer = 54,
            SipPortType = 55,
            SipAuthenticationUser = 56,
            SipAuthenticationPassword = 57,
            SipTransportMethod = 58,
            SipPrivateCert = 59,
            SipLocalCert = 60,
            SipCACerts = 61,
            NumCACerts = 62,
            ProxyStatus = 63,
            SendKey = 64,
            DialTimeout = 65,
            InterDigitShortTimer = 66,
            InterDigitLongTimer = 67,
            DialPlanString = 68,
            DialPlanRuleString = 69,
            ExtensionLength = 70,
            LocalMinDigits = 71,
            LocalMaxDigits = 72,
            LongDistMinDigits = 73,
            LongDistMaxDigits = 74,
            InternationalMinDigits = 75,
            InternationalMaxDigits = 76,
            EnetProv = 77,
            EnetTFTPIP = 78,
            DomainName = 79,
            SignalGeneratorType = 80,
            Amplitude = 81,
            SweepRate = 82,
            SweepRepeat = 83,
            AdaptiveVolumeRatio = 84,
            FeedbackMode = 85,
            NumberFixedFeedbackNodes = 86,
            FeedbackBandwidth = 87,
            FeedbackDepth = 88,
            NoiseGateMode = 89,
            NoiseGateTimer = 90,
            NoiseGateFilter = 91,
            SoundMaskMode = 92,
            SoundMaskTimer = 93,
            EqFilter = 94,
            Polarity = 95,
            Impedance = 96,
            HDEchoCancelMode = 97,
            BFMode = 98,
            BFZoneEnabled = 99,
            NotchSuccess = 100,
            NotchUse = 101,
            Time = 102,
            Number = 103,
            VLanPriority = 104,
            VLanID = 105,
            QosPrecedence = 106,
            QosCustomDSCP = 107,
            SerialNumber = 108,
            BaudRate = 109,
            FlowControl = 110,
            CountryCode = 111,
            ErrorLogMask = 112,
            Command = 113,
            TimeZone = 114,
            SNMPUserId = 115,
            SNMPUserAuthorizationKey = 116,
            SNMPPrivacyKey = 117,
            MacAddress = 118,
            ChannelID = 119,
            BinaryBlobLength = 120,
            BinaryBlobData = 121,
            AdaptiveVolumeReferenceChannel = 122,
            AdaptiveVolumeReferenceObject = 123,
            LastMic = 124,
            MaxMics = 125,
            GatingGroupMode = 126,
            VirtualReferenceOutput = 127,
            MatrixStatus = 128,
            Persistence = 129,
            ConductorPriority = 130,
            BundleNumber = 131,
            BundleType = 132,
            BundleBitDepth = 133,
            BundleLatency = 134,
            BundleMaxUnicast = 135,
            StreamID = 136,
            StreamMap = 137,
            StreamState = 138,
            NumberTalkers = 139,
            NumberListeners = 140,
            Linked = 141,
            StreamStatus = 142,
            ObjectID = 143,
            Status = 144,
            TotalSize = 145,
            Type = 146,
            HasValidData = 147,
            SuccessStatus = 148,
            MeterGranularity = 149,
            MeterType = 150,
            Width = 151,
            Height = 152,
            Index = 153,
            ID = 154,
            X = 155,
            Y = 156,
            BaseObject = 157,
            PropertyID = 158,
            PropertyTypeID = 159,
            SystemLog = 160,
            Mute,
            FeedbackEnable,
        }

		public enum EOverriddenProperties
		{
			ChannelName = 65536,
			ChannelMute = 65537,
			ChannelGainMax = 65538,
			ChannelGainMin = 65539,
			AutomaticGainControlEnable = 65536,
			AutomaticGainControlID = 65537,
			MicrophoneAdaptiveAmbientEnable = 65536,
			MicrophoneChairmanOverrideEnable = 65537,
			MicrophoneEchoCancelEnable = 65538,
			MicrophonePhantomPowerEnable = 65539,
			MicrophonePowerAmpAdaptiveModeEnable = 65540,
			MicrophonePushToTalkEnable = 65541,
			MicrophoneSpeechLevelEnable = 65542,
			MicrophoneCoarseGain = 65543,
			MicrophonePushToTalkThreshold = 65544,
			OutputNumberOpenMicsEnable = 65536,
			TelcoReceiveAudibleConnectEnable = 65536,
			TelcoReceiveAutoAnswerEnable = 65537,
			TelcoReceiveClearEffectEnable = 65538,
			TelcoReceiveEchoCancellationEnable = 65539,
			TelcoReceiveReceiveBoostEnable = 65540,
			TelcoReceiveReceiveReductionEnable = 65541,
			TelcoReceiveRingerEnable = 65542,
			TelcoReceiveTelcoLevelControlEnable = 65544,
			TelcoReceiveAudibleConnectLevel = 65545,
			TelcoReceiveDialToneLevel = 65546,
			TelcoReceiveDTMFLevel = 65547,
			TelcoReceiveReceiveBoostLevel = 65548,
			TelcoReceiveRingerLevel = 65549,
			TelcoReceiveLastNumberDialed = 65550,
			TelcoReceiveLocalNumber = 65551,
			UsbTransmitName = 65536,
			UsbTransmitMute = 65537,
			UsbTransmitNumberOpenMicsEnable = 65538,
			UsbTransmitGain = 65539,
			UsbTransmitGainMax = 65540,
			UsbTransmitGainMin = 65541,
			FilterEnable = 65536,
			FilterID = 65537,
			CompressorEnable = 65536,
			CompressorGain = 65538,
			CompressorThreshold = 65539,
			CompressorGateRatio = 65540,
			CompressorID = 65541,
			EthernetValuesDHCPEnable = 65536,
			EthernetValuesDNSEnable = 65537,
			EthernetValuesDomainName = 65538,
			EthernetValuesDNSAddress1 = 65539,
			EthernetValuesDNSAddress2 = 65540,
			EthernetValuesGateway = 65541,
			EthernetValuesSubnetMask = 65542,
			EthernetValuesID = 65543,
			UsbReceiveName = 65536,
			UsbReceiveMute = 65537,
			UsbReceiveGain = 65538,
			UsbReceiveGainMax = 65539,
			UsbReceiveGainMin = 65540,
			VoipReceiveAudibleConnectEnable = 65536,
			VoipReceiveAutoAnswerEnable = 65537,
			VoipReceiveClearEffectEnable = 65538,
			VoipReceiveRingerEnable = 65539,
			VoipReceiveAudibleConnectLevel = 65540,
			VoipReceiveDialToneLevel = 65541,
			VoipReceiveDTMFLevel = 65542,
			VoipReceiveRingerLevel = 65543,
			VoipReceivePriorityNumbers = 65544,
			VoipReceiveCallWaitingEnable = 65545,
			VoipReceivePriorityAnswerEnable = 65546,
			VoipCommonSipRTCPEnable = 65536,
			VoipCommonVADEnable = 65537,
			VoipCommonVLanEnabled = 65538,
			VoipCommonSipRTPBasePort = 65539,
			VoipCommonLocalNumber = 65540,
			VoipCommonLastNumberDialed = 65541,
			VoipCommonCallControlLog = 65542,
			VoipCommonSIPLog = 65543,
			VoipCommonID = 65544,
			VoipProxySipAuthenticationEnable = 65536,
			VoipProxyFQDN = 65537,
			VoipProxyID = 65538,
			VoipDialPlanID = 65536,
			SignalGeneratorEnable = 65536,
			SignalGeneratorSweepEndFrequency = 65537,
			SignalGeneratorSweepIncrementFrequency = 65538,
			SignalGeneratorSweepStartFrequency = 65539,
			SignalGeneratorID = 65540,
			AdaptiveVolumeEnable = 65536,
			AdaptiveVolumeGain = 65537,
			AdaptiveVolumeThreshold = 65538,
			AdaptiveVolumeID = 65539,
			FeedbackEliminationFeedbackEnable = 65536,
			FeedbackEliminationFeedbackRingEnable = 65537,
			FeedbackEliminationFeedbackGain = 65538,
			FeedbackEliminationID = 65539,
			NoiseGateEnable = 65536,
			NoiseGateThreshold = 65537,
			NoiseGateID = 65538,
			SoundMaskEnable = 65536,
			SoundMaskLevel = 65537,
			SoundMaskID = 65538,
			DelayEnable = 65536,
			DelayID = 65537,
			GraphicEqualizerEnable = 65536,
			GraphicEqualizerID = 65537,
			PowerAmpSoftClipEnable = 65536,
			LimiterEnable = 65536,
			LimiterThreshold = 65537,
			LimiterID = 65538,
			BeamFormingMicrophoneAdaptiveAmbientEnable = 65536,
			BeamFormingMicrophoneChairmanOverrideEnable = 65537,
			BeamFormingMicrophoneSpeechLevelEnable = 65538,
			BeamFormingMicrophoneHDEchoCancelEnable = 65539,
			BeamFormingMicrophoneHDReferenceSelectionChannel_3 = 65540,
			BeamFormingMicrophoneHDReferenceSelectionChannel_1 = 65541,
			BeamFormingMicrophoneHDReferenceSelectionChannel_2 = 65542,
			BeamFormingMicrophoneHDReferenceSelectionObject_3 = 65543,
			BeamFormingMicrophoneHDReferenceSelectionObject_1 = 65544,
			BeamFormingMicrophoneHDReferenceSelectionObject_2 = 65545,
			BeamFormingMicrophoneMuteCommandDeviceID = 65546,
			BeamFormingMicrophoneDeviceID = 65547,
			BeamFormingMicrophoneMuteCommandDeviceTypeID = 65548,
			BeamFormingMicrophoneMuteOffCommand = 65549,
			BeamFormingMicrophoneMuteOnCommand = 65550,
			VersionMajor = 65536,
			VersionMinor = 65537,
			VersionRevision = 65538,
			VersionBuild = 65539,
			VersionID = 65540,
			TimedEventEndTime = 65536,
			TimedEventStartTime = 65537,
			TimedEventNumberOccurrances = 65538,
			TimedEventRepeatCount = 65539,
			TimedEventStartDate = 65540,
			TimedEventEndDate = 65541,
			TimedEventReoccurPattern = 65542,
			TimedEventReoccurDays = 65543,
			TimedEventID = 65544,
			DeviceTelnetEnable = 65536,
			DeviceSerialEchoEnable = 65537,
			DeviceTelnetPort = 65538,
			DeviceIPMask = 65539,
			DeviceDeviceID = 65540,
			GlobalSystemTimeZoneName = 65536,
			GlobalSystemSystemName = 65537,
			GlobalSystemCountryString = 65538,
			GlobalSystemStateString = 65539,
			GlobalSystemCityString = 65540,
			GlobalSystemCompanyString = 65541,
			GlobalSystemBuildingString = 65542,
			GlobalSystemRegionString = 65543,
			GlobalSystemDaylightSavings = 65544,
			GlobalSystemTimeServer1 = 65545,
			GlobalSystemTimeServer2 = 65546,
			GlobalSystemID = 65547,
			SNMPSettingsContext = 65536,
			SNMPSettingsPrivateCommunity = 65537,
			SNMPSettingsPublicCommunity = 65538,
			SNMPSettingsUserName = 65539,
			SNMPSettingsAuthenticationEnable = 65540,
			SNMPSettingsPrivacyEnable = 65541,
			SNMPSettingsManagerPort = 65542,
			SNMPSettingsManagerIP = 65543,
			SNMPSettingsID = 65544,
			GateEnable = 65536,
			GateID = 65537,
			NoiseCancellationEnable = 65536,
			NoiseCancellationID = 65537,
			VoipTLSZippedCertsLength = 65536,
			VoipTLSZippedCerts = 65537,
			VoipTLSID = 65538,
			VoipOutboundProxyEnable = 65536,
			VoipOutboundProxyID = 65537,
			PowerAmpCommonEnergySaverEnable = 65536,
			PowerAmpCommonID = 65537,
			GatingGroupFirstMicPriorityEnable = 65536,
			GatingGroupID = 65537,
			VirtualReferenceID = 65536,
			GPIOHighCommand = 65536,
			GPIOLowCommand = 65537,
			GPIOID = 65538,
			MatrixCrossPointInputType = 65536,
			MatrixCrossPointOutputType = 65537,
			MatrixCrossPointInputChannelID = 65538,
			MatrixCrossPointOutputChannelID = 65539,
			MatrixCrossPointID = 65540,
			MatrixColumns = 65536,
			MatrixRows = 65537,
			MatrixID = 65538,
			CommandCommandID = 65536,
			CommandCommandLength = 65537,
			CobraNetContact = 65536,
			CobraNetLocation = 65537,
			CobraNetRxBundleNumber = 65538,
			CobraNetRxBundleType = 65539,
			CobraNetTxBundleNumber = 65540,
			CobraNetTxBundleType = 65541,
			CobraNetRxMacAddress = 65542,
			CobraNetTxMacAddress = 65543,
			CobraNetSerialEnable = 65544,
			AVBStreamNumberChannels = 65536,
			AVBStreamID = 65537,
			VTableCount = 65536,
			VTableAuthenticationPort = 65537,
			VTableDeviceGLinkOrder = 65536,
			VTableDeviceDeviceType = 65537,
			VTableDeviceNumberBeamFormers = 65538,
			VTableDeviceNumberCobraNets = 65539,
			VTableDeviceNumberAVBs = 65540,
			PhoneBookEntrySpeedDial = 65536,
			PhoneBookEntryID = 65537,
			LocationIDCount = 65536,
			LocationIDVersionNumber = 65537,
			ConnectionUserName = 65536,
			ConnectionPassword = 65537,
			ConnectionCommandPort = 65538,
			ConnectionMeterPort = 65539,
			ConnectionFirmwarePort = 65540,
		}

		public enum EObjects
		{
			Channel = 1,
			Input = 2,
			AutomaticGainControl = 3,
			Microphone = 4,
			Output = 5,
			TelcoReceive = 6,
			TelcoTransmit = 7,
			VoipTransmit = 8,
			UsbTransmit = 9,
			Filter = 10,
			Compressor = 11,
			Submix = 12,
			Fader = 13,
			EthernetValues = 14,
			UsbReceive = 15,
			VoipReceive = 16,
			VoipCommon = 17,
			VoipProxy = 18,
			VoipDialPlan = 19,
			SignalGenerator = 20,
			AdaptiveVolume = 21,
			FeedbackElimination = 22,
			NoiseGate = 23,
			SoundMask = 24,
			Delay = 25,
			GraphicEqualizer = 26,
			PowerAmp = 27,
			Limiter = 28,
			BeamFormingMicrophone = 29,
			Version = 30,
			TimedEvent = 31,
			Device = 32,
			GlobalSystem = 33,
			SNMPSettings = 34,
			Gate = 35,
			NoiseCancellation = 36,
			VoipTLS = 37,
			VoipOutboundProxy = 38,
			PowerAmpCommon = 39,
			GLink = 40,
			GatingGroup = 41,
			VirtualReference = 42,
			GPIO = 43,
			MatrixCrossPoint = 44,
			Matrix = 45,
			Command = 46,
			AVB = 47,
			ClassDOutput = 48,
			Ducker = 49,
			CobraNet = 50,
			AVBStream = 51,
			VTable = 52,
			VTableDevice = 53,
			PhoneBookEntry = 54,
			PhoneBook = 55,
			LocationID = 56,
			IDNode = 57,
			Connection = 58,
			TimedEvents = 59,
			Meter = 60,
		}

        //public enum EDevices
        //{
        //    Timpanogos8x8 = 0,
        //    TimpanogosVoip = 1,
        //    Timpanogos8x8Amp = 2,
        //    Timpanogos8x8Plus = 3,
        //    TimpanogosEmpty = 4,
        //}

        //public enum ERooms
        //{
        //    AudioConfiguration = 1,
        //    Subroom = 2,
        //    PhysicalComponent = 3,
        //    Divider = 4,
        //    PhysicalRoom = 5,
        //    CrossPoint = 6,
        //    LogicalComponent = 7,
        //    ComponentGroup = 8,
        //}

        //public const UInt16 Name_Max = 256;
        //public const UInt16 Name_Min = 1;
        //public const byte OnOffToggle_Default = 0;
        //public const byte OnOffToggle_Increment = 1;
        //public const byte OnOffToggle_Max = 2;
        //public const byte OnOffToggle_Min = 0;
        //public const float Gain_Default = 0F;
        //public const float Gain_Increment = 0.5F;
        //public const float Gain_Max = 20F;
        //public const float Gain_Min = -65F;
        //public const UInt16 Port_Default = 0;
        //public const UInt16 Port_Increment = 1;
        //public const UInt16 Port_Max = 65535;
        //public const UInt16 Port_Min = 0;
        //public const UInt16 PhoneNumber_Max = 45;
        //public const UInt16 PhoneNumber_Min = 0;
        //public const float Threshold_Default = 0F;
        //public const float Threshold_Increment = 0.5F;
        //public const float Threshold_Max = 0F;
        //public const float Threshold_Min = -50F;
        //public const float Target_Default = 0F;
        //public const float Target_Increment = 0.5F;
        //public const float Target_Max = 20F;
        //public const float Target_Min = -30F;
        //public const UInt16 Attack_Default = 100;
        //public const UInt16 Attack_Increment = 10;
        //public const UInt16 Attack_Max = 10000;
        //public const UInt16 Attack_Min = 100;
        //public const byte GateRatio_Default = 0;
        //public const byte GateRatio_Increment = 1;
        //public const byte GateRatio_Max = 50;
        //public const byte GateRatio_Min = 0;
        //public const byte GateMode_Default = 0;
        //public const byte GateMode_Increment = 1;
        //public const byte GateMode_Max = 2;
        //public const byte GateMode_Min = 0;
        //public const float GateHold_Default = 0.100000001490116F;
        //public const float GateHold_Increment = 0.100000001490116F;
        //public const float GateHold_Max = 8F;
        //public const float GateHold_Min = 0.100000001490116F;
        //public const byte OffAttenuation_Default = 0;
        //public const byte OffAttenuation_Increment = 1;
        //public const byte OffAttenuation_Max = 60;
        //public const byte OffAttenuation_Min = 0;
        //public const byte DecayRate_Default = 1;
        //public const byte DecayRate_Increment = 1;
        //public const byte DecayRate_Max = 3;
        //public const byte DecayRate_Min = 1;
        //public const byte MixerSelection_Default = 1;
        //public const byte MixerSelection_Increment = 1;
        //public const byte MixerSelection_Max = 10;
        //public const byte MixerSelection_Min = 1;
        //public const float AmbientLevel_Default = 0F;
        //public const float AmbientLevel_Increment = 0.5F;
        //public const float AmbientLevel_Max = 0F;
        //public const float AmbientLevel_Min = -80F;
        //public const byte ReferenceSelectionChannel_Default = 1;
        //public const byte ReferenceSelectionChannel_Increment = 1;
        //public const byte ReferenceSelectionChannel_Max = 20;
        //public const byte ReferenceSelectionChannel_Min = 1;
        //public const byte ReferenceSelectionObject_Default = 0;
        //public const byte ReferenceSelectionObject_Increment = 1;
        //public const byte ReferenceSelectionObject_Max = 255;
        //public const byte ReferenceSelectionObject_Min = 0;
        //public const byte NonLinearProcessing_Default = 0;
        //public const byte NonLinearProcessing_Increment = 1;
        //public const byte NonLinearProcessing_Max = 3;
        //public const byte NonLinearProcessing_Min = 0;
        //public const byte NoiseCancellationLevel_Default = 6;
        //public const byte NoiseCancellationLevel_Increment = 1;
        //public const byte NoiseCancellationLevel_Max = 15;
        //public const byte NoiseCancellationLevel_Min = 6;
        //public const byte TelcoAdaptMode_Default = 0;
        //public const byte TelcoAdaptMode_Increment = 1;
        //public const byte TelcoAdaptMode_Max = 1;
        //public const byte TelcoAdaptMode_Min = 0;
        //public const byte AutoAnswerRings_Default = 2;
        //public const byte AutoAnswerRings_Increment = 1;
        //public const byte AutoAnswerRings_Max = 4;
        //public const byte AutoAnswerRings_Min = 2;
        //public const byte RingerSelection_Default = 1;
        //public const byte RingerSelection_Increment = 1;
        //public const byte RingerSelection_Max = 3;
        //public const byte RingerSelection_Min = 1;
        //public const byte AutoDisconnect_Default = 0;
        //public const byte AutoDisconnect_Increment = 1;
        //public const byte AutoDisconnect_Max = 3;
        //public const byte AutoDisconnect_Min = 0;
        //public const UInt16 HookFlashDuration_Default = 50;
        //public const UInt16 HookFlashDuration_Increment = 10;
        //public const UInt16 HookFlashDuration_Max = 2000;
        //public const UInt16 HookFlashDuration_Min = 50;
        //public const byte LastSpeedDial_Default = 0;
        //public const byte LastSpeedDial_Increment = 1;
        //public const byte LastSpeedDial_Max = 20;
        //public const byte LastSpeedDial_Min = 0;
        //public const UInt16 FQDN_Max = 80;
        //public const UInt16 FQDN_Min = 0;
        //public const byte RingCadence_Default = 0;
        //public const byte RingCadence_Increment = 1;
        //public const byte RingCadence_Max = 1;
        //public const byte RingCadence_Min = 0;
        //public const UInt16 RingOnTime_Default = 100;
        //public const UInt16 RingOnTime_Increment = 10;
        //public const UInt16 RingOnTime_Max = 1020;
        //public const UInt16 RingOnTime_Min = 100;
        //public const UInt16 RingOffTime_Default = 80;
        //public const UInt16 RingOffTime_Increment = 10;
        //public const UInt16 RingOffTime_Max = 1020;
        //public const UInt16 RingOffTime_Min = 80;
        //public const UInt32 Frequency_Default = 20;
        //public const UInt32 Frequency_Increment = 100;
        //public const UInt32 Frequency_Max = 20000;
        //public const UInt32 Frequency_Min = 20;
        //public const float GainSlope_Default = 0F;
        //public const float GainSlope_Increment = 0.5F;
        //public const float GainSlope_Max = 24F;
        //public const float GainSlope_Min = -80F;
        //public const float Bandwidth_Default = 0.0500000007450581F;
        //public const float Bandwidth_Increment = 0.0500000007450581F;
        //public const float Bandwidth_Max = 5F;
        //public const float Bandwidth_Min = 0.0500000007450581F;
        //public const byte FilterType_Default = 0;
        //public const byte FilterType_Increment = 1;
        //public const byte FilterType_Max = 11;
        //public const byte FilterType_Min = 0;
        //public const byte CompressorRatio_Default = 1;
        //public const byte CompressorRatio_Increment = 1;
        //public const byte CompressorRatio_Max = 20;
        //public const byte CompressorRatio_Min = 1;
        //public const UInt16 Release_Default = 100;
        //public const UInt16 Release_Increment = 1;
        //public const UInt16 Release_Max = 2000;
        //public const UInt16 Release_Min = 100;
        //public const UInt16 DelayTime_Default = 0;
        //public const UInt16 DelayTime_Increment = 1;
        //public const UInt16 DelayTime_Max = 5000;
        //public const UInt16 DelayTime_Min = 0;
        //public const UInt32 IPAddress_Default = 0;
        //public const UInt32 IPAddress_Increment = 1;
        //public const UInt32 IPAddress_Max = 4294967040;
        //public const UInt32 IPAddress_Min = 0;
        //public const UInt16 SipNoAnswerTimeout_Default = 18000;
        //public const UInt16 SipNoAnswerTimeout_Increment = 1;
        //public const UInt16 SipNoAnswerTimeout_Max = 18000;
        //public const UInt16 SipNoAnswerTimeout_Min = 0;
        //public const UInt16 SipRetransTimer_Default = 0;
        //public const UInt16 SipRetransTimer_Increment = 1;
        //public const UInt16 SipRetransTimer_Max = 65535;
        //public const UInt16 SipRetransTimer_Min = 0;
        //public const byte RTPRTCPLog_Default = 0;
        //public const byte RTPRTCPLog_Increment = 1;
        //public const byte RTPRTCPLog_Max = 15;
        //public const byte RTPRTCPLog_Min = 0;
        //public const UInt16 SipRetranTimer4_Default = 0;
        //public const UInt16 SipRetranTimer4_Increment = 1;
        //public const UInt16 SipRetranTimer4_Max = 65535;
        //public const UInt16 SipRetranTimer4_Min = 0;
        //public const byte SipSRTPEnable_Default = 0;
        //public const byte SipSRTPEnable_Increment = 1;
        //public const byte SipSRTPEnable_Max = 2;
        //public const byte SipSRTPEnable_Min = 0;
        //public const byte SipSRTPCipher_Default = 0;
        //public const byte SipSRTPCipher_Increment = 1;
        //public const byte SipSRTPCipher_Max = 2;
        //public const byte SipSRTPCipher_Min = 0;
        //public const byte SipSRTPMac_Default = 0;
        //public const byte SipSRTPMac_Increment = 1;
        //public const byte SipSRTPMac_Max = 2;
        //public const byte SipSRTPMac_Min = 0;
        //public const byte SipSRTPKdr_Default = 0;
        //public const byte SipSRTPKdr_Increment = 1;
        //public const byte SipSRTPKdr_Max = 24;
        //public const byte SipSRTPKdr_Min = 0;
        //public const byte SipSRTCPEnable_Default = 0;
        //public const byte SipSRTCPEnable_Increment = 1;
        //public const byte SipSRTCPEnable_Max = 2;
        //public const byte SipSRTCPEnable_Min = 0;
        //public const byte DTMFRelayEnable_Default = 0;
        //public const byte DTMFRelayEnable_Increment = 1;
        //public const byte DTMFRelayEnable_Max = 2;
        //public const byte DTMFRelayEnable_Min = 0;
        //public const byte DTMFPayload_Default = 96;
        //public const byte DTMFPayload_Increment = 1;
        //public const byte DTMFPayload_Max = 127;
        //public const byte DTMFPayload_Min = 96;
        //public const byte CodecPriority_Default = 0;
        //public const byte CodecPriority_Increment = 1;
        //public const byte CodecPriority_Max = 255;
        //public const byte CodecPriority_Min = 0;
        //public const int CodecPriority_Count = 17;
        //public const float VADThreshold_Default = 0F;
        //public const float VADThreshold_Increment = 0.5F;
        //public const float VADThreshold_Max = 10F;
        //public const float VADThreshold_Min = -20F;
        //public const byte ProxyType_Default = 0;
        //public const byte ProxyType_Increment = 1;
        //public const byte ProxyType_Max = 1;
        //public const byte ProxyType_Min = 0;
        //public const byte VADNoiseMatching_Default = 0;
        //public const byte VADNoiseMatching_Increment = 1;
        //public const byte VADNoiseMatching_Max = 2;
        //public const byte VADNoiseMatching_Min = 0;
        //public const UInt16 SipSessionTimer_Default = 0;
        //public const UInt16 SipSessionTimer_Increment = 1;
        //public const UInt16 SipSessionTimer_Max = 65535;
        //public const UInt16 SipSessionTimer_Min = 0;
        //public const UInt16 SipMinSETimer_Default = 0;
        //public const UInt16 SipMinSETimer_Increment = 1;
        //public const UInt16 SipMinSETimer_Max = 65535;
        //public const UInt16 SipMinSETimer_Min = 0;
        //public const UInt16 SipRegistrationTimer_Default = 0;
        //public const UInt16 SipRegistrationTimer_Increment = 1;
        //public const UInt16 SipRegistrationTimer_Max = 65535;
        //public const UInt16 SipRegistrationTimer_Min = 0;
        //public const byte SipPortType_Default = 0;
        //public const byte SipPortType_Increment = 1;
        //public const byte SipPortType_Max = 2;
        //public const byte SipPortType_Min = 0;
        //public const UInt16 SipAuthenticationUser_Max = 50;
        //public const UInt16 SipAuthenticationUser_Min = 0;
        //public const UInt16 SipAuthenticationPassword_Max = 16;
        //public const UInt16 SipAuthenticationPassword_Min = 0;
        //public const byte SipTransportMethod_Default = 0;
        //public const byte SipTransportMethod_Increment = 1;
        //public const byte SipTransportMethod_Max = 2;
        //public const byte SipTransportMethod_Min = 0;
        //public const UInt16 SipPrivateCert_Max = 64;
        //public const UInt16 SipPrivateCert_Min = 0;
        //public const UInt16 SipLocalCert_Max = 64;
        //public const UInt16 SipLocalCert_Min = 0;
        //public const UInt16 SipCACerts_Max = 64;
        //public const UInt16 SipCACerts_Min = 0;
        //public const int SipCACerts_Count = 5;
        //public const byte NumCACerts_Default = 0;
        //public const byte NumCACerts_Increment = 1;
        //public const byte NumCACerts_Max = 5;
        //public const byte NumCACerts_Min = 0;
        //public const byte ProxyStatus_Default = 0;
        //public const byte ProxyStatus_Increment = 1;
        //public const byte ProxyStatus_Max = 2;
        //public const byte ProxyStatus_Min = 0;
        //public const byte SendKey_Default = 0;
        //public const byte SendKey_Increment = 1;
        //public const byte SendKey_Max = 2;
        //public const byte SendKey_Min = 0;
        //public const byte DialTimeout_Default = 20;
        //public const byte DialTimeout_Increment = 1;
        //public const byte DialTimeout_Max = 30;
        //public const byte DialTimeout_Min = 1;
        //public const byte InterDigitShortTimer_Default = 4;
        //public const byte InterDigitShortTimer_Increment = 1;
        //public const byte InterDigitShortTimer_Max = 20;
        //public const byte InterDigitShortTimer_Min = 1;
        //public const byte InterDigitLongTimer_Default = 16;
        //public const byte InterDigitLongTimer_Increment = 1;
        //public const byte InterDigitLongTimer_Max = 20;
        //public const byte InterDigitLongTimer_Min = 1;
        //public const UInt16 DialPlanString_Max = 1000;
        //public const UInt16 DialPlanString_Min = 1;
        //public const UInt16 DialPlanRuleString_Max = 1000;
        //public const UInt16 DialPlanRuleString_Min = 0;
        //public const byte ExtensionLength_Default = 3;
        //public const byte ExtensionLength_Increment = 1;
        //public const byte ExtensionLength_Max = 15;
        //public const byte ExtensionLength_Min = 3;
        //public const byte LocalMinDigits_Default = 1;
        //public const byte LocalMinDigits_Increment = 1;
        //public const byte LocalMinDigits_Max = 44;
        //public const byte LocalMinDigits_Min = 1;
        //public const byte LocalMaxDigits_Default = 1;
        //public const byte LocalMaxDigits_Increment = 1;
        //public const byte LocalMaxDigits_Max = 44;
        //public const byte LocalMaxDigits_Min = 1;
        //public const byte LongDistMinDigits_Default = 1;
        //public const byte LongDistMinDigits_Increment = 1;
        //public const byte LongDistMinDigits_Max = 44;
        //public const byte LongDistMinDigits_Min = 1;
        //public const byte LongDistMaxDigits_Default = 1;
        //public const byte LongDistMaxDigits_Increment = 1;
        //public const byte LongDistMaxDigits_Max = 44;
        //public const byte LongDistMaxDigits_Min = 1;
        //public const byte InternationalMinDigits_Default = 1;
        //public const byte InternationalMinDigits_Increment = 1;
        //public const byte InternationalMinDigits_Max = 44;
        //public const byte InternationalMinDigits_Min = 1;
        //public const byte InternationalMaxDigits_Default = 1;
        //public const byte InternationalMaxDigits_Increment = 1;
        //public const byte InternationalMaxDigits_Max = 44;
        //public const byte InternationalMaxDigits_Min = 1;
        //public const byte EnetProv_Default = 0;
        //public const byte EnetProv_Increment = 1;
        //public const byte EnetProv_Max = 2;
        //public const byte EnetProv_Min = 0;
        //public const UInt32 EnetTFTPIP_Default = 0;
        //public const UInt32 EnetTFTPIP_Increment = 1;
        //public const UInt32 EnetTFTPIP_Max = 4294967040;
        //public const UInt32 EnetTFTPIP_Min = 0;
        //public const UInt16 DomainName_Max = 255;
        //public const UInt16 DomainName_Min = 0;
        //public const byte SignalGeneratorType_Default = 1;
        //public const byte SignalGeneratorType_Increment = 1;
        //public const byte SignalGeneratorType_Max = 4;
        //public const byte SignalGeneratorType_Min = 1;
        //public const float Amplitude_Default = 0F;
        //public const float Amplitude_Increment = 1F;
        //public const float Amplitude_Max = 20F;
        //public const float Amplitude_Min = -60F;
        //public const UInt16 SweepRate_Default = 10;
        //public const UInt16 SweepRate_Increment = 100;
        //public const UInt16 SweepRate_Max = 20000;
        //public const UInt16 SweepRate_Min = 10;
        //public const byte SweepRepeat_Default = 0;
        //public const byte SweepRepeat_Increment = 1;
        //public const byte SweepRepeat_Max = 1;
        //public const byte SweepRepeat_Min = 0;
        //public const byte AdaptiveVolumeRatio_Default = 0;
        //public const byte AdaptiveVolumeRatio_Increment = 1;
        //public const byte AdaptiveVolumeRatio_Max = 3;
        //public const byte AdaptiveVolumeRatio_Min = 0;
        //public const byte FeedbackMode_Default = 0;
        //public const byte FeedbackMode_Increment = 1;
        //public const byte FeedbackMode_Max = 1;
        //public const byte FeedbackMode_Min = 0;
        //public const byte NumberFixedFeedbackNodes_Default = 0;
        //public const byte NumberFixedFeedbackNodes_Increment = 1;
        //public const byte NumberFixedFeedbackNodes_Max = 24;
        //public const byte NumberFixedFeedbackNodes_Min = 0;
        //public const byte FeedbackBandwidth_Default = 0;
        //public const byte FeedbackBandwidth_Increment = 1;
        //public const byte FeedbackBandwidth_Max = 1;
        //public const byte FeedbackBandwidth_Min = 0;
        //public const byte FeedbackDepth_Default = 0;
        //public const byte FeedbackDepth_Increment = 1;
        //public const byte FeedbackDepth_Max = 1;
        //public const byte FeedbackDepth_Min = 0;
        //public const byte NoiseGateMode_Default = 0;
        //public const byte NoiseGateMode_Increment = 1;
        //public const byte NoiseGateMode_Max = 1;
        //public const byte NoiseGateMode_Min = 0;
        //public const byte NoiseGateTimer_Default = 1;
        //public const byte NoiseGateTimer_Increment = 1;
        //public const byte NoiseGateTimer_Max = 16;
        //public const byte NoiseGateTimer_Min = 1;
        //public const byte NoiseGateFilter_Default = 0;
        //public const byte NoiseGateFilter_Increment = 1;
        //public const byte NoiseGateFilter_Max = 3;
        //public const byte NoiseGateFilter_Min = 0;
        //public const byte SoundMaskMode_Default = 0;
        //public const byte SoundMaskMode_Increment = 1;
        //public const byte SoundMaskMode_Max = 1;
        //public const byte SoundMaskMode_Min = 0;
        //public const UInt16 SoundMaskTimer_Default = 0;
        //public const UInt16 SoundMaskTimer_Increment = 1;
        //public const UInt16 SoundMaskTimer_Max = 1440;
        //public const UInt16 SoundMaskTimer_Min = 0;
        //public const float EqFilter_Default = 0F;
        //public const float EqFilter_Increment = 0.5F;
        //public const float EqFilter_Max = 12F;
        //public const float EqFilter_Min = -12F;
        //public const int EqFilter_Count = 10;
        //public const byte Polarity_Default = 0;
        //public const byte Polarity_Increment = 1;
        //public const byte Polarity_Max = 2;
        //public const byte Polarity_Min = 0;
        //public const byte Impedance_Default = 0;
        //public const byte Impedance_Increment = 1;
        //public const byte Impedance_Max = 1;
        //public const byte Impedance_Min = 0;
        //public const byte HDEchoCancelMode_Default = 0;
        //public const byte HDEchoCancelMode_Increment = 1;
        //public const byte HDEchoCancelMode_Max = 3;
        //public const byte HDEchoCancelMode_Min = 0;
        //public const byte BFMode_Default = 1;
        //public const byte BFMode_Increment = 1;
        //public const byte BFMode_Max = 4;
        //public const byte BFMode_Min = 1;
        //public const byte BFZoneEnabled_Default = 0;
        //public const byte BFZoneEnabled_Increment = 1;
        //public const byte BFZoneEnabled_Max = 2;
        //public const byte BFZoneEnabled_Min = 0;
        //public const int BFZoneEnabled_Count = 8;
        //public const byte NotchSuccess_Default = 0;
        //public const byte NotchSuccess_Increment = 1;
        //public const byte NotchSuccess_Max = 2;
        //public const byte NotchSuccess_Min = 0;
        //public const byte NotchUse_Default = 0;
        //public const byte NotchUse_Increment = 1;
        //public const byte NotchUse_Max = 2;
        //public const byte NotchUse_Min = 0;
        //public const UInt32 Time_Default = 0;
        //public const UInt32 Time_Increment = 1;
        //public const UInt32 Time_Max = 4294967040;
        //public const UInt32 Time_Min = 0;
        //public const byte Number_Default = 0;
        //public const byte Number_Increment = 1;
        //public const byte Number_Max = 255;
        //public const byte Number_Min = 0;
        //public const byte VLanPriority_Default = 0;
        //public const byte VLanPriority_Increment = 1;
        //public const byte VLanPriority_Max = 7;
        //public const byte VLanPriority_Min = 0;
        //public const UInt16 VLanID_Default = 1;
        //public const UInt16 VLanID_Increment = 1;
        //public const UInt16 VLanID_Max = 4094;
        //public const UInt16 VLanID_Min = 1;
        //public const byte QosPrecedence_Default = 0;
        //public const byte QosPrecedence_Increment = 1;
        //public const byte QosPrecedence_Max = 8;
        //public const byte QosPrecedence_Min = 0;
        //public const byte QosCustomDSCP_Default = 0;
        //public const byte QosCustomDSCP_Increment = 1;
        //public const byte QosCustomDSCP_Max = 63;
        //public const byte QosCustomDSCP_Min = 0;
        //public const UInt32 SerialNumber_Default = 0;
        //public const UInt32 SerialNumber_Increment = 1;
        //public const UInt32 SerialNumber_Max = 4294967040;
        //public const UInt32 SerialNumber_Min = 0;
        //public const byte BaudRate_Default = 4;
        //public const byte BaudRate_Increment = 1;
        //public const byte BaudRate_Max = 4;
        //public const byte BaudRate_Min = 0;
        //public const byte FlowControl_Default = 0;
        //public const byte FlowControl_Increment = 1;
        //public const byte FlowControl_Max = 2;
        //public const byte FlowControl_Min = 0;
        //public const byte CountryCode_Default = 1;
        //public const byte CountryCode_Increment = 1;
        //public const byte CountryCode_Max = 16;
        //public const byte CountryCode_Min = 1;
        //public const UInt32 ErrorLogMask_Default = 0;
        //public const UInt32 ErrorLogMask_Increment = 1;
        //public const UInt32 ErrorLogMask_Max = 4294967040;
        //public const UInt32 ErrorLogMask_Min = 0;
        //public const UInt32 Command_Default = 0;
        //public const UInt32 Command_Increment = 1;
        //public const UInt32 Command_Max = 1048576;
        //public const UInt32 Command_Min = 0;
        //public const Int16 TimeZone_Default = 0;
        //public const Int16 TimeZone_Increment = 1;
        //public const Int16 TimeZone_Max = 24;
        //public const Int16 TimeZone_Min = -24;
        //public const byte SNMPUserId_Default = 0;
        //public const byte SNMPUserId_Increment = 1;
        //public const byte SNMPUserId_Max = 255;
        //public const byte SNMPUserId_Min = 0;
        //public const int SNMPUserId_Count = 12;
        //public const byte SNMPUserAuthorizationKey_Default = 0;
        //public const byte SNMPUserAuthorizationKey_Increment = 1;
        //public const byte SNMPUserAuthorizationKey_Max = 255;
        //public const byte SNMPUserAuthorizationKey_Min = 0;
        //public const int SNMPUserAuthorizationKey_Count = 16;
        //public const byte SNMPPrivacyKey_Default = 0;
        //public const byte SNMPPrivacyKey_Increment = 1;
        //public const byte SNMPPrivacyKey_Max = 255;
        //public const byte SNMPPrivacyKey_Min = 0;
        //public const int SNMPPrivacyKey_Count = 20;
        //public const UInt64 MacAddress_Default = 0;
        //public const UInt64 MacAddress_Increment = 1;
        //public const UInt64 MacAddress_Max = 281475010265088;
        //public const UInt64 MacAddress_Min = 0;
        //public const byte ChannelID_Default = 1;
        //public const byte ChannelID_Increment = 1;
        //public const byte ChannelID_Max = 255;
        //public const byte ChannelID_Min = 1;
        //public const UInt32 BinaryBlobLength_Default = 0;
        //public const UInt32 BinaryBlobLength_Increment = 0;
        //public const UInt32 BinaryBlobLength_Max = 20971520;
        //public const UInt32 BinaryBlobLength_Min = 0;
        //public const UInt32 BinaryBlobData_Default = 0;
        //public const UInt32 BinaryBlobData_Increment = 1;
        //public const UInt32 BinaryBlobData_Max = 20971520;
        //public const UInt32 BinaryBlobData_Min = 0;
        //public const byte AdaptiveVolumeReferenceChannel_Default = 0;
        //public const byte AdaptiveVolumeReferenceChannel_Increment = 1;
        //public const byte AdaptiveVolumeReferenceChannel_Max = 255;
        //public const byte AdaptiveVolumeReferenceChannel_Min = 0;
        //public const byte AdaptiveVolumeReferenceObject_Default = 0;
        //public const byte AdaptiveVolumeReferenceObject_Increment = 1;
        //public const byte AdaptiveVolumeReferenceObject_Max = 255;
        //public const byte AdaptiveVolumeReferenceObject_Min = 0;
        //public const byte LastMic_Default = 0;
        //public const byte LastMic_Increment = 1;
        //public const byte LastMic_Max = 255;
        //public const byte LastMic_Min = 0;
        //public const byte MaxMics_Default = 0;
        //public const byte MaxMics_Increment = 1;
        //public const byte MaxMics_Max = 255;
        //public const byte MaxMics_Min = 0;
        //public const byte GatingGroupMode_Default = 0;
        //public const byte GatingGroupMode_Increment = 1;
        //public const byte GatingGroupMode_Max = 255;
        //public const byte GatingGroupMode_Min = 0;
        //public const byte VirtualReferenceOutput_Default = 0;
        //public const byte VirtualReferenceOutput_Increment = 1;
        //public const byte VirtualReferenceOutput_Max = 255;
        //public const byte VirtualReferenceOutput_Min = 0;
        //public const byte MatrixStatus_Default = 0;
        //public const byte MatrixStatus_Increment = 1;
        //public const byte MatrixStatus_Max = 255;
        //public const byte MatrixStatus_Min = 0;
        //public const byte Persistence_Default = 0;
        //public const byte Persistence_Increment = 1;
        //public const byte Persistence_Max = 1;
        //public const byte Persistence_Min = 0;
        //public const byte ConductorPriority_Default = 32;
        //public const byte ConductorPriority_Increment = 1;
        //public const byte ConductorPriority_Max = 255;
        //public const byte ConductorPriority_Min = 0;
        //public const UInt16 BundleNumber_Default = 0;
        //public const UInt16 BundleNumber_Increment = 1;
        //public const UInt16 BundleNumber_Max = 65535;
        //public const UInt16 BundleNumber_Min = 0;
        //public const byte BundleType_Default = 0;
        //public const byte BundleType_Increment = 1;
        //public const byte BundleType_Max = 1;
        //public const byte BundleType_Min = 0;
        //public const byte BundleBitDepth_Default = 1;
        //public const byte BundleBitDepth_Increment = 1;
        //public const byte BundleBitDepth_Max = 2;
        //public const byte BundleBitDepth_Min = 0;
        //public const byte BundleLatency_Default = 0;
        //public const byte BundleLatency_Increment = 1;
        //public const byte BundleLatency_Max = 2;
        //public const byte BundleLatency_Min = 0;
        //public const byte BundleMaxUnicast_Default = 1;
        //public const byte BundleMaxUnicast_Increment = 1;
        //public const byte BundleMaxUnicast_Max = 4;
        //public const byte BundleMaxUnicast_Min = 1;
        //public const UInt64 StreamID_Default = 0;
        //public const UInt64 StreamID_Increment = 1;
        //public const UInt64 StreamID_Max = 18446739675663040512;
        //public const UInt64 StreamID_Min = 0;
        //public const UInt64 StreamMap_Default = 0;
        //public const UInt64 StreamMap_Increment = 1;
        //public const UInt64 StreamMap_Max = 18446739675663040512;
        //public const UInt64 StreamMap_Min = 0;
        //public const UInt32 StreamState_Default = 0;
        //public const UInt32 StreamState_Increment = 1;
        //public const UInt32 StreamState_Max = 4294967040;
        //public const UInt32 StreamState_Min = 0;
        //public const byte NumberTalkers_Default = 0;
        //public const byte NumberTalkers_Increment = 1;
        //public const byte NumberTalkers_Max = 4;
        //public const byte NumberTalkers_Min = 0;
        //public const byte NumberListeners_Default = 0;
        //public const byte NumberListeners_Increment = 1;
        //public const byte NumberListeners_Max = 4;
        //public const byte NumberListeners_Min = 0;
        //public const byte Linked_Default = 0;
        //public const byte Linked_Increment = 1;
        //public const byte Linked_Max = 1;
        //public const byte Linked_Min = 0;
        //public const UInt32 StreamStatus_Default = 0;
        //public const UInt32 StreamStatus_Increment = 1;
        //public const UInt32 StreamStatus_Max = 4294967040;
        //public const UInt32 StreamStatus_Min = 0;
        //public const UInt16 ObjectID_Default = 1;
        //public const UInt16 ObjectID_Increment = 1;
        //public const UInt16 ObjectID_Max = 65535;
        //public const UInt16 ObjectID_Min = 1;
        //public const UInt32 Status_Default = 0;
        //public const UInt32 Status_Increment = 1;
        //public const UInt32 Status_Max = 4294967040;
        //public const UInt32 Status_Min = 0;
        //public const UInt32 TotalSize_Default = 0;
        //public const UInt32 TotalSize_Increment = 1;
        //public const UInt32 TotalSize_Max = 4294967040;
        //public const UInt32 TotalSize_Min = 0;
        //public const UInt16 Type_Default = 0;
        //public const UInt16 Type_Increment = 1;
        //public const UInt16 Type_Max = 65535;
        //public const UInt16 Type_Min = 0;
        //public const byte MeterGranularity_Default = 0;
        //public const byte MeterGranularity_Increment = 1;
        //public const byte MeterGranularity_Max = 2;
        //public const byte MeterGranularity_Min = 0;
        //public const byte MeterType_Default = 0;
        //public const byte MeterType_Increment = 1;
        //public const byte MeterType_Max = 11;
        //public const byte MeterType_Min = 0;
        //public const UInt16 Width_Default = 100;
        //public const UInt16 Width_Increment = 1;
        //public const UInt16 Width_Max = 65535;
        //public const UInt16 Width_Min = 1;
        //public const UInt16 Height_Default = 100;
        //public const UInt16 Height_Increment = 1;
        //public const UInt16 Height_Max = 65535;
        //public const UInt16 Height_Min = 1;
        //public const byte Index_Default = 0;
        //public const byte Index_Increment = 1;
        //public const byte Index_Max = 255;
        //public const byte Index_Min = 0;
        //public const UInt16 ID_Default = 1;
        //public const UInt16 ID_Increment = 1;
        //public const UInt16 ID_Max = 65535;
        //public const UInt16 ID_Min = 1;
        //public const UInt16 X_Default = 1;
        //public const UInt16 X_Increment = 1;
        //public const UInt16 X_Max = 65535;
        //public const UInt16 X_Min = 1;
        //public const UInt16 Y_Default = 1;
        //public const UInt16 Y_Increment = 1;
        //public const UInt16 Y_Max = 65535;
        //public const UInt16 Y_Min = 1;
        //public const UInt32 PropertyID_Default = 0;
        //public const UInt32 PropertyID_Increment = 1;
        //public const UInt32 PropertyID_Max = 4294967040;
        //public const UInt32 PropertyID_Min = 0;
        //public const byte PropertyTypeID_Default = 0;
        //public const byte PropertyTypeID_Increment = 1;
        //public const byte PropertyTypeID_Max = 255;
        //public const byte PropertyTypeID_Min = 0;
        //public const UInt32 SystemLog_Default = 0;
        //public const UInt32 SystemLog_Increment = 1;
        //public const UInt32 SystemLog_Max = 1048576;
        //public const UInt32 SystemLog_Min = 0;
        //public const UInt16 ChannelName_Max = 64;
        //public const UInt16 ChannelName_Min = 1;
        //public const byte ChannelMute_Default = 0;
        //public const byte ChannelMute_Increment = 1;
        //public const byte ChannelMute_Max = 2;
        //public const byte ChannelMute_Min = 0;
        //public const float ChannelGainMax_Default = 20F;
        //public const float ChannelGainMax_Increment = 0.5F;
        //public const float ChannelGainMax_Max = 20F;
        //public const float ChannelGainMax_Min = -65F;
        //public const float ChannelGainMin_Default = -65F;
        //public const float ChannelGainMin_Increment = 0.5F;
        //public const float ChannelGainMin_Max = 20F;
        //public const float ChannelGainMin_Min = -65F;
        //public const byte AutomaticGainControlEnable_Default = 0;
        //public const byte AutomaticGainControlEnable_Increment = 1;
        //public const byte AutomaticGainControlEnable_Max = 2;
        //public const byte AutomaticGainControlEnable_Min = 0;
        //public const byte AutomaticGainControlID_Default = 1;
        //public const byte AutomaticGainControlID_Increment = 1;
        //public const byte AutomaticGainControlID_Max = 255;
        //public const byte AutomaticGainControlID_Min = 1;
        //public const byte MicrophoneAdaptiveAmbientEnable_Default = 0;
        //public const byte MicrophoneAdaptiveAmbientEnable_Increment = 1;
        //public const byte MicrophoneAdaptiveAmbientEnable_Max = 2;
        //public const byte MicrophoneAdaptiveAmbientEnable_Min = 0;
        //public const byte MicrophoneChairmanOverrideEnable_Default = 0;
        //public const byte MicrophoneChairmanOverrideEnable_Increment = 1;
        //public const byte MicrophoneChairmanOverrideEnable_Max = 2;
        //public const byte MicrophoneChairmanOverrideEnable_Min = 0;
        //public const byte MicrophoneEchoCancelEnable_Default = 0;
        //public const byte MicrophoneEchoCancelEnable_Increment = 1;
        //public const byte MicrophoneEchoCancelEnable_Max = 2;
        //public const byte MicrophoneEchoCancelEnable_Min = 0;
        //public const byte MicrophonePhantomPowerEnable_Default = 0;
        //public const byte MicrophonePhantomPowerEnable_Increment = 1;
        //public const byte MicrophonePhantomPowerEnable_Max = 2;
        //public const byte MicrophonePhantomPowerEnable_Min = 0;
        //public const byte MicrophonePowerAmpAdaptiveModeEnable_Default = 0;
        //public const byte MicrophonePowerAmpAdaptiveModeEnable_Increment = 1;
        //public const byte MicrophonePowerAmpAdaptiveModeEnable_Max = 2;
        //public const byte MicrophonePowerAmpAdaptiveModeEnable_Min = 0;
        //public const byte MicrophonePushToTalkEnable_Default = 0;
        //public const byte MicrophonePushToTalkEnable_Increment = 1;
        //public const byte MicrophonePushToTalkEnable_Max = 2;
        //public const byte MicrophonePushToTalkEnable_Min = 0;
        //public const byte MicrophoneSpeechLevelEnable_Default = 0;
        //public const byte MicrophoneSpeechLevelEnable_Increment = 1;
        //public const byte MicrophoneSpeechLevelEnable_Max = 2;
        //public const byte MicrophoneSpeechLevelEnable_Min = 0;
        //public const byte MicrophoneCoarseGain_Default = 0;
        //public const byte MicrophoneCoarseGain_Increment = 7;
        //public const byte MicrophoneCoarseGain_Max = 56;
        //public const byte MicrophoneCoarseGain_Min = 0;
        //public const float MicrophonePushToTalkThreshold_Default = 0F;
        //public const float MicrophonePushToTalkThreshold_Increment = 0.5F;
        //public const float MicrophonePushToTalkThreshold_Max = 0F;
        //public const float MicrophonePushToTalkThreshold_Min = -100F;
        //public const UInt16 MicrophoneFilter_Count = 8;
        //public const byte OutputNumberOpenMicsEnable_Default = 0;
        //public const byte OutputNumberOpenMicsEnable_Increment = 1;
        //public const byte OutputNumberOpenMicsEnable_Max = 2;
        //public const byte OutputNumberOpenMicsEnable_Min = 0;
        //public const byte TelcoReceiveAudibleConnectEnable_Default = 0;
        //public const byte TelcoReceiveAudibleConnectEnable_Increment = 1;
        //public const byte TelcoReceiveAudibleConnectEnable_Max = 2;
        //public const byte TelcoReceiveAudibleConnectEnable_Min = 0;
        //public const byte TelcoReceiveAutoAnswerEnable_Default = 0;
        //public const byte TelcoReceiveAutoAnswerEnable_Increment = 1;
        //public const byte TelcoReceiveAutoAnswerEnable_Max = 2;
        //public const byte TelcoReceiveAutoAnswerEnable_Min = 0;
        //public const byte TelcoReceiveClearEffectEnable_Default = 0;
        //public const byte TelcoReceiveClearEffectEnable_Increment = 1;
        //public const byte TelcoReceiveClearEffectEnable_Max = 2;
        //public const byte TelcoReceiveClearEffectEnable_Min = 0;
        //public const byte TelcoReceiveEchoCancellationEnable_Default = 0;
        //public const byte TelcoReceiveEchoCancellationEnable_Increment = 1;
        //public const byte TelcoReceiveEchoCancellationEnable_Max = 2;
        //public const byte TelcoReceiveEchoCancellationEnable_Min = 0;
        //public const byte TelcoReceiveReceiveBoostEnable_Default = 0;
        //public const byte TelcoReceiveReceiveBoostEnable_Increment = 1;
        //public const byte TelcoReceiveReceiveBoostEnable_Max = 2;
        //public const byte TelcoReceiveReceiveBoostEnable_Min = 0;
        //public const byte TelcoReceiveReceiveReductionEnable_Default = 0;
        //public const byte TelcoReceiveReceiveReductionEnable_Increment = 1;
        //public const byte TelcoReceiveReceiveReductionEnable_Max = 2;
        //public const byte TelcoReceiveReceiveReductionEnable_Min = 0;
        //public const byte TelcoReceiveRingerEnable_Default = 0;
        //public const byte TelcoReceiveRingerEnable_Increment = 1;
        //public const byte TelcoReceiveRingerEnable_Max = 2;
        //public const byte TelcoReceiveRingerEnable_Min = 0;
        //public const byte TelcoReceiveTelcoLevelControlEnable_Default = 0;
        //public const byte TelcoReceiveTelcoLevelControlEnable_Increment = 1;
        //public const byte TelcoReceiveTelcoLevelControlEnable_Max = 2;
        //public const byte TelcoReceiveTelcoLevelControlEnable_Min = 0;
        //public const float TelcoReceiveAudibleConnectLevel_Default = 0F;
        //public const float TelcoReceiveAudibleConnectLevel_Increment = 0.5F;
        //public const float TelcoReceiveAudibleConnectLevel_Max = 12F;
        //public const float TelcoReceiveAudibleConnectLevel_Min = -12F;
        //public const float TelcoReceiveDialToneLevel_Default = 0F;
        //public const float TelcoReceiveDialToneLevel_Increment = 0.5F;
        //public const float TelcoReceiveDialToneLevel_Max = 12F;
        //public const float TelcoReceiveDialToneLevel_Min = -12F;
        //public const float TelcoReceiveDTMFLevel_Default = 0F;
        //public const float TelcoReceiveDTMFLevel_Increment = 0.5F;
        //public const float TelcoReceiveDTMFLevel_Max = 12F;
        //public const float TelcoReceiveDTMFLevel_Min = -12F;
        //public const float TelcoReceiveReceiveBoostLevel_Default = 0F;
        //public const float TelcoReceiveReceiveBoostLevel_Increment = 0.5F;
        //public const float TelcoReceiveReceiveBoostLevel_Max = 12F;
        //public const float TelcoReceiveReceiveBoostLevel_Min = 0F;
        //public const float TelcoReceiveRingerLevel_Default = 0F;
        //public const float TelcoReceiveRingerLevel_Increment = 0.5F;
        //public const float TelcoReceiveRingerLevel_Max = 12F;
        //public const float TelcoReceiveRingerLevel_Min = -12F;
        //public const UInt16 TelcoReceiveLastNumberDialed_Max = 45;
        //public const UInt16 TelcoReceiveLastNumberDialed_Min = 0;
        //public const UInt16 TelcoReceiveLocalNumber_Max = 16;
        //public const UInt16 TelcoReceiveLocalNumber_Min = 0;
        //public const UInt16 UsbTransmitName_Max = 64;
        //public const UInt16 UsbTransmitName_Min = 1;
        //public const byte UsbTransmitMute_Default = 0;
        //public const byte UsbTransmitMute_Increment = 1;
        //public const byte UsbTransmitMute_Max = 2;
        //public const byte UsbTransmitMute_Min = 0;
        //public const byte UsbTransmitNumberOpenMicsEnable_Default = 0;
        //public const byte UsbTransmitNumberOpenMicsEnable_Increment = 1;
        //public const byte UsbTransmitNumberOpenMicsEnable_Max = 2;
        //public const byte UsbTransmitNumberOpenMicsEnable_Min = 0;
        //public const float UsbTransmitGain_Default = 12F;
        //public const float UsbTransmitGain_Increment = 0.5F;
        //public const float UsbTransmitGain_Max = 12F;
        //public const float UsbTransmitGain_Min = -12F;
        //public const float UsbTransmitGainMax_Default = 12F;
        //public const float UsbTransmitGainMax_Increment = 0.5F;
        //public const float UsbTransmitGainMax_Max = 12F;
        //public const float UsbTransmitGainMax_Min = -12F;
        //public const float UsbTransmitGainMin_Default = -12F;
        //public const float UsbTransmitGainMin_Increment = 0.5F;
        //public const float UsbTransmitGainMin_Max = 12F;
        //public const float UsbTransmitGainMin_Min = -12F;
        //public const byte FilterEnable_Default = 0;
        //public const byte FilterEnable_Increment = 1;
        //public const byte FilterEnable_Max = 2;
        //public const byte FilterEnable_Min = 0;
        //public const byte FilterID_Default = 1;
        //public const byte FilterID_Increment = 1;
        //public const byte FilterID_Max = 255;
        //public const byte FilterID_Min = 1;
        //public const byte CompressorEnable_Default = 0;
        //public const byte CompressorEnable_Increment = 1;
        //public const byte CompressorEnable_Max = 2;
        //public const byte CompressorEnable_Min = 0;
        //public const float CompressorGain_Default = 0F;
        //public const float CompressorGain_Increment = 0.5F;
        //public const float CompressorGain_Max = 20F;
        //public const float CompressorGain_Min = 0F;
        //public const float CompressorThreshold_Default = 0F;
        //public const float CompressorThreshold_Increment = 0.5F;
        //public const float CompressorThreshold_Max = 20F;
        //public const float CompressorThreshold_Min = -30F;
        //public const byte CompressorGateRatio_Default = 0;
        //public const byte CompressorGateRatio_Increment = 1;
        //public const byte CompressorGateRatio_Max = 100;
        //public const byte CompressorGateRatio_Min = 0;
        //public const byte CompressorID_Default = 1;
        //public const byte CompressorID_Increment = 1;
        //public const byte CompressorID_Max = 255;
        //public const byte CompressorID_Min = 1;
        //public const UInt16 SubmixFilter_Count = 4;
        //public const byte EthernetValuesDHCPEnable_Default = 0;
        //public const byte EthernetValuesDHCPEnable_Increment = 1;
        //public const byte EthernetValuesDHCPEnable_Max = 2;
        //public const byte EthernetValuesDHCPEnable_Min = 0;
        //public const byte EthernetValuesDNSEnable_Default = 0;
        //public const byte EthernetValuesDNSEnable_Increment = 1;
        //public const byte EthernetValuesDNSEnable_Max = 2;
        //public const byte EthernetValuesDNSEnable_Min = 0;
        //public const UInt16 EthernetValuesDomainName_Max = 255;
        //public const UInt16 EthernetValuesDomainName_Min = 0;
        //public const UInt32 EthernetValuesDNSAddress1_Default = 0;
        //public const UInt32 EthernetValuesDNSAddress1_Increment = 1;
        //public const UInt32 EthernetValuesDNSAddress1_Max = 4294967040;
        //public const UInt32 EthernetValuesDNSAddress1_Min = 0;
        //public const UInt32 EthernetValuesDNSAddress2_Default = 0;
        //public const UInt32 EthernetValuesDNSAddress2_Increment = 1;
        //public const UInt32 EthernetValuesDNSAddress2_Max = 4294967040;
        //public const UInt32 EthernetValuesDNSAddress2_Min = 0;
        //public const UInt32 EthernetValuesGateway_Default = 0;
        //public const UInt32 EthernetValuesGateway_Increment = 1;
        //public const UInt32 EthernetValuesGateway_Max = 4294967040;
        //public const UInt32 EthernetValuesGateway_Min = 0;
        //public const UInt32 EthernetValuesSubnetMask_Default = 0;
        //public const UInt32 EthernetValuesSubnetMask_Increment = 1;
        //public const UInt32 EthernetValuesSubnetMask_Max = 4294967040;
        //public const UInt32 EthernetValuesSubnetMask_Min = 0;
        //public const byte EthernetValuesID_Default = 1;
        //public const byte EthernetValuesID_Increment = 1;
        //public const byte EthernetValuesID_Max = 255;
        //public const byte EthernetValuesID_Min = 1;
        //public const UInt16 UsbReceiveName_Max = 64;
        //public const UInt16 UsbReceiveName_Min = 1;
        //public const byte UsbReceiveMute_Default = 0;
        //public const byte UsbReceiveMute_Increment = 1;
        //public const byte UsbReceiveMute_Max = 2;
        //public const byte UsbReceiveMute_Min = 0;
        //public const float UsbReceiveGain_Default = 12F;
        //public const float UsbReceiveGain_Increment = 0.5F;
        //public const float UsbReceiveGain_Max = 12F;
        //public const float UsbReceiveGain_Min = -12F;
        //public const float UsbReceiveGainMax_Default = 12F;
        //public const float UsbReceiveGainMax_Increment = 0.5F;
        //public const float UsbReceiveGainMax_Max = 12F;
        //public const float UsbReceiveGainMax_Min = -12F;
        //public const float UsbReceiveGainMin_Default = -12F;
        //public const float UsbReceiveGainMin_Increment = 0.5F;
        //public const float UsbReceiveGainMin_Max = 12F;
        //public const float UsbReceiveGainMin_Min = -12F;
        //public const byte VoipReceiveAudibleConnectEnable_Default = 0;
        //public const byte VoipReceiveAudibleConnectEnable_Increment = 1;
        //public const byte VoipReceiveAudibleConnectEnable_Max = 2;
        //public const byte VoipReceiveAudibleConnectEnable_Min = 0;
        //public const byte VoipReceiveAutoAnswerEnable_Default = 0;
        //public const byte VoipReceiveAutoAnswerEnable_Increment = 1;
        //public const byte VoipReceiveAutoAnswerEnable_Max = 2;
        //public const byte VoipReceiveAutoAnswerEnable_Min = 0;
        //public const byte VoipReceiveClearEffectEnable_Default = 0;
        //public const byte VoipReceiveClearEffectEnable_Increment = 1;
        //public const byte VoipReceiveClearEffectEnable_Max = 2;
        //public const byte VoipReceiveClearEffectEnable_Min = 0;
        //public const byte VoipReceiveRingerEnable_Default = 0;
        //public const byte VoipReceiveRingerEnable_Increment = 1;
        //public const byte VoipReceiveRingerEnable_Max = 2;
        //public const byte VoipReceiveRingerEnable_Min = 0;
        //public const float VoipReceiveAudibleConnectLevel_Default = 0F;
        //public const float VoipReceiveAudibleConnectLevel_Increment = 0.5F;
        //public const float VoipReceiveAudibleConnectLevel_Max = 12F;
        //public const float VoipReceiveAudibleConnectLevel_Min = -12F;
        //public const float VoipReceiveDialToneLevel_Default = 0F;
        //public const float VoipReceiveDialToneLevel_Increment = 0.5F;
        //public const float VoipReceiveDialToneLevel_Max = 12F;
        //public const float VoipReceiveDialToneLevel_Min = -12F;
        //public const float VoipReceiveDTMFLevel_Default = 0F;
        //public const float VoipReceiveDTMFLevel_Increment = 0.5F;
        //public const float VoipReceiveDTMFLevel_Max = 12F;
        //public const float VoipReceiveDTMFLevel_Min = -12F;
        //public const float VoipReceiveRingerLevel_Default = 0F;
        //public const float VoipReceiveRingerLevel_Increment = 0.5F;
        //public const float VoipReceiveRingerLevel_Max = 12F;
        //public const float VoipReceiveRingerLevel_Min = -12F;
        //public const UInt16 VoipReceivePriorityNumbers_Max = 45;
        //public const UInt16 VoipReceivePriorityNumbers_Min = 0;
        //public const int VoipReceivePriorityNumbers_Count = 20;
        //public const byte VoipReceiveCallWaitingEnable_Default = 0;
        //public const byte VoipReceiveCallWaitingEnable_Increment = 1;
        //public const byte VoipReceiveCallWaitingEnable_Max = 2;
        //public const byte VoipReceiveCallWaitingEnable_Min = 0;
        //public const byte VoipReceivePriorityAnswerEnable_Default = 0;
        //public const byte VoipReceivePriorityAnswerEnable_Increment = 1;
        //public const byte VoipReceivePriorityAnswerEnable_Max = 2;
        //public const byte VoipReceivePriorityAnswerEnable_Min = 0;
        //public const byte VoipCommonSipRTCPEnable_Default = 0;
        //public const byte VoipCommonSipRTCPEnable_Increment = 1;
        //public const byte VoipCommonSipRTCPEnable_Max = 2;
        //public const byte VoipCommonSipRTCPEnable_Min = 0;
        //public const byte VoipCommonVADEnable_Default = 0;
        //public const byte VoipCommonVADEnable_Increment = 1;
        //public const byte VoipCommonVADEnable_Max = 2;
        //public const byte VoipCommonVADEnable_Min = 0;
        //public const byte VoipCommonVLanEnabled_Default = 0;
        //public const byte VoipCommonVLanEnabled_Increment = 1;
        //public const byte VoipCommonVLanEnabled_Max = 2;
        //public const byte VoipCommonVLanEnabled_Min = 0;
        //public const Int32 VoipCommonSipRTPBasePort_Default = 1024;
        //public const Int32 VoipCommonSipRTPBasePort_Increment = 1;
        //public const Int32 VoipCommonSipRTPBasePort_Max = 65535;
        //public const Int32 VoipCommonSipRTPBasePort_Min = 1024;
        //public const UInt16 VoipCommonLocalNumber_Max = 16;
        //public const UInt16 VoipCommonLocalNumber_Min = 0;
        //public const UInt16 VoipCommonLastNumberDialed_Max = 45;
        //public const UInt16 VoipCommonLastNumberDialed_Min = 0;
        //public const UInt16 VoipCommonCallControlLog_Max = 5000;
        //public const UInt16 VoipCommonCallControlLog_Min = 0;
        //public const UInt16 VoipCommonSIPLog_Max = 5000;
        //public const UInt16 VoipCommonSIPLog_Min = 0;
        //public const byte VoipCommonID_Default = 1;
        //public const byte VoipCommonID_Increment = 1;
        //public const byte VoipCommonID_Max = 255;
        //public const byte VoipCommonID_Min = 1;
        //public const byte VoipProxySipAuthenticationEnable_Default = 0;
        //public const byte VoipProxySipAuthenticationEnable_Increment = 1;
        //public const byte VoipProxySipAuthenticationEnable_Max = 2;
        //public const byte VoipProxySipAuthenticationEnable_Min = 0;
        //public const UInt16 VoipProxyFQDN_Max = 80;
        //public const UInt16 VoipProxyFQDN_Min = 0;
        //public const byte VoipProxyID_Default = 1;
        //public const byte VoipProxyID_Increment = 1;
        //public const byte VoipProxyID_Max = 255;
        //public const byte VoipProxyID_Min = 1;
        //public const byte VoipDialPlanID_Default = 1;
        //public const byte VoipDialPlanID_Increment = 1;
        //public const byte VoipDialPlanID_Max = 255;
        //public const byte VoipDialPlanID_Min = 1;
        //public const byte SignalGeneratorEnable_Default = 0;
        //public const byte SignalGeneratorEnable_Increment = 1;
        //public const byte SignalGeneratorEnable_Max = 2;
        //public const byte SignalGeneratorEnable_Min = 0;
        //public const UInt16 SignalGeneratorSweepEndFrequency_Default = 0;
        //public const UInt16 SignalGeneratorSweepEndFrequency_Increment = 100;
        //public const UInt16 SignalGeneratorSweepEndFrequency_Max = 22000;
        //public const UInt16 SignalGeneratorSweepEndFrequency_Min = 20;
        //public const UInt16 SignalGeneratorSweepIncrementFrequency_Default = 0;
        //public const UInt16 SignalGeneratorSweepIncrementFrequency_Increment = 100;
        //public const UInt16 SignalGeneratorSweepIncrementFrequency_Max = 22000;
        //public const UInt16 SignalGeneratorSweepIncrementFrequency_Min = 1;
        //public const UInt16 SignalGeneratorSweepStartFrequency_Default = 0;
        //public const UInt16 SignalGeneratorSweepStartFrequency_Increment = 100;
        //public const UInt16 SignalGeneratorSweepStartFrequency_Max = 22000;
        //public const UInt16 SignalGeneratorSweepStartFrequency_Min = 20;
        //public const byte SignalGeneratorID_Default = 1;
        //public const byte SignalGeneratorID_Increment = 1;
        //public const byte SignalGeneratorID_Max = 255;
        //public const byte SignalGeneratorID_Min = 1;
        //public const byte AdaptiveVolumeEnable_Default = 0;
        //public const byte AdaptiveVolumeEnable_Increment = 1;
        //public const byte AdaptiveVolumeEnable_Max = 2;
        //public const byte AdaptiveVolumeEnable_Min = 0;
        //public const float AdaptiveVolumeGain_Default = 0F;
        //public const float AdaptiveVolumeGain_Increment = 0.5F;
        //public const float AdaptiveVolumeGain_Max = 20F;
        //public const float AdaptiveVolumeGain_Min = -100F;
        //public const float AdaptiveVolumeThreshold_Default = 0F;
        //public const float AdaptiveVolumeThreshold_Increment = 0.5F;
        //public const float AdaptiveVolumeThreshold_Max = 20F;
        //public const float AdaptiveVolumeThreshold_Min = -100F;
        //public const byte AdaptiveVolumeID_Default = 1;
        //public const byte AdaptiveVolumeID_Increment = 1;
        //public const byte AdaptiveVolumeID_Max = 255;
        //public const byte AdaptiveVolumeID_Min = 1;
        //public const byte FeedbackEliminationFeedbackEnable_Default = 0;
        //public const byte FeedbackEliminationFeedbackEnable_Increment = 1;
        //public const byte FeedbackEliminationFeedbackEnable_Max = 2;
        //public const byte FeedbackEliminationFeedbackEnable_Min = 0;
        //public const byte FeedbackEliminationFeedbackRingEnable_Default = 0;
        //public const byte FeedbackEliminationFeedbackRingEnable_Increment = 1;
        //public const byte FeedbackEliminationFeedbackRingEnable_Max = 2;
        //public const byte FeedbackEliminationFeedbackRingEnable_Min = 0;
        //public const float FeedbackEliminationFeedbackGain_Default = 0F;
        //public const float FeedbackEliminationFeedbackGain_Increment = 0.5F;
        //public const float FeedbackEliminationFeedbackGain_Max = 8F;
        //public const float FeedbackEliminationFeedbackGain_Min = 0F;
        //public const byte FeedbackEliminationID_Default = 1;
        //public const byte FeedbackEliminationID_Increment = 1;
        //public const byte FeedbackEliminationID_Max = 255;
        //public const byte FeedbackEliminationID_Min = 1;
        //public const byte NoiseGateEnable_Default = 0;
        //public const byte NoiseGateEnable_Increment = 1;
        //public const byte NoiseGateEnable_Max = 2;
        //public const byte NoiseGateEnable_Min = 0;
        //public const float NoiseGateThreshold_Default = 0F;
        //public const float NoiseGateThreshold_Increment = 0.5F;
        //public const float NoiseGateThreshold_Max = 0F;
        //public const float NoiseGateThreshold_Min = -100F;
        //public const byte NoiseGateID_Default = 1;
        //public const byte NoiseGateID_Increment = 1;
        //public const byte NoiseGateID_Max = 255;
        //public const byte NoiseGateID_Min = 1;
        //public const byte SoundMaskEnable_Default = 0;
        //public const byte SoundMaskEnable_Increment = 1;
        //public const byte SoundMaskEnable_Max = 2;
        //public const byte SoundMaskEnable_Min = 0;
        //public const float SoundMaskLevel_Default = 0F;
        //public const float SoundMaskLevel_Increment = 0.5F;
        //public const float SoundMaskLevel_Max = 20F;
        //public const float SoundMaskLevel_Min = -65F;
        //public const byte SoundMaskID_Default = 1;
        //public const byte SoundMaskID_Increment = 1;
        //public const byte SoundMaskID_Max = 255;
        //public const byte SoundMaskID_Min = 1;
        //public const byte DelayEnable_Default = 0;
        //public const byte DelayEnable_Increment = 1;
        //public const byte DelayEnable_Max = 2;
        //public const byte DelayEnable_Min = 0;
        //public const byte DelayID_Default = 1;
        //public const byte DelayID_Increment = 1;
        //public const byte DelayID_Max = 255;
        //public const byte DelayID_Min = 1;
        //public const byte GraphicEqualizerEnable_Default = 0;
        //public const byte GraphicEqualizerEnable_Increment = 1;
        //public const byte GraphicEqualizerEnable_Max = 2;
        //public const byte GraphicEqualizerEnable_Min = 0;
        //public const byte GraphicEqualizerID_Default = 1;
        //public const byte GraphicEqualizerID_Increment = 1;
        //public const byte GraphicEqualizerID_Max = 255;
        //public const byte GraphicEqualizerID_Min = 1;
        //public const byte PowerAmpSoftClipEnable_Default = 0;
        //public const byte PowerAmpSoftClipEnable_Increment = 1;
        //public const byte PowerAmpSoftClipEnable_Max = 2;
        //public const byte PowerAmpSoftClipEnable_Min = 0;
        //public const byte LimiterEnable_Default = 0;
        //public const byte LimiterEnable_Increment = 1;
        //public const byte LimiterEnable_Max = 2;
        //public const byte LimiterEnable_Min = 0;
        //public const float LimiterThreshold_Default = 0F;
        //public const float LimiterThreshold_Increment = 0.5F;
        //public const float LimiterThreshold_Max = 20F;
        //public const float LimiterThreshold_Min = -65F;
        //public const byte LimiterID_Default = 1;
        //public const byte LimiterID_Increment = 1;
        //public const byte LimiterID_Max = 255;
        //public const byte LimiterID_Min = 1;
        //public const byte BeamFormingMicrophoneAdaptiveAmbientEnable_Default = 0;
        //public const byte BeamFormingMicrophoneAdaptiveAmbientEnable_Increment = 1;
        //public const byte BeamFormingMicrophoneAdaptiveAmbientEnable_Max = 2;
        //public const byte BeamFormingMicrophoneAdaptiveAmbientEnable_Min = 0;
        //public const byte BeamFormingMicrophoneChairmanOverrideEnable_Default = 0;
        //public const byte BeamFormingMicrophoneChairmanOverrideEnable_Increment = 1;
        //public const byte BeamFormingMicrophoneChairmanOverrideEnable_Max = 2;
        //public const byte BeamFormingMicrophoneChairmanOverrideEnable_Min = 0;
        //public const byte BeamFormingMicrophoneSpeechLevelEnable_Default = 0;
        //public const byte BeamFormingMicrophoneSpeechLevelEnable_Increment = 1;
        //public const byte BeamFormingMicrophoneSpeechLevelEnable_Max = 2;
        //public const byte BeamFormingMicrophoneSpeechLevelEnable_Min = 0;
        //public const byte BeamFormingMicrophoneHDEchoCancelEnable_Default = 0;
        //public const byte BeamFormingMicrophoneHDEchoCancelEnable_Increment = 1;
        //public const byte BeamFormingMicrophoneHDEchoCancelEnable_Max = 2;
        //public const byte BeamFormingMicrophoneHDEchoCancelEnable_Min = 0;
        //public const byte BeamFormingMicrophoneHDReferenceSelectionChannel_3_Default = 1;
        //public const byte BeamFormingMicrophoneHDReferenceSelectionChannel_3_Increment = 1;
        //public const byte BeamFormingMicrophoneHDReferenceSelectionChannel_3_Max = 20;
        //public const byte BeamFormingMicrophoneHDReferenceSelectionChannel_3_Min = 1;
        //public const byte BeamFormingMicrophoneHDReferenceSelectionChannel_1_Default = 1;
        //public const byte BeamFormingMicrophoneHDReferenceSelectionChannel_1_Increment = 1;
        //public const byte BeamFormingMicrophoneHDReferenceSelectionChannel_1_Max = 20;
        //public const byte BeamFormingMicrophoneHDReferenceSelectionChannel_1_Min = 1;
        //public const byte BeamFormingMicrophoneHDReferenceSelectionChannel_2_Default = 1;
        //public const byte BeamFormingMicrophoneHDReferenceSelectionChannel_2_Increment = 1;
        //public const byte BeamFormingMicrophoneHDReferenceSelectionChannel_2_Max = 20;
        //public const byte BeamFormingMicrophoneHDReferenceSelectionChannel_2_Min = 1;
        //public const byte BeamFormingMicrophoneHDReferenceSelectionObject_3_Default = 0;
        //public const byte BeamFormingMicrophoneHDReferenceSelectionObject_3_Increment = 1;
        //public const byte BeamFormingMicrophoneHDReferenceSelectionObject_3_Max = 255;
        //public const byte BeamFormingMicrophoneHDReferenceSelectionObject_3_Min = 0;
        //public const byte BeamFormingMicrophoneHDReferenceSelectionObject_1_Default = 0;
        //public const byte BeamFormingMicrophoneHDReferenceSelectionObject_1_Increment = 1;
        //public const byte BeamFormingMicrophoneHDReferenceSelectionObject_1_Max = 255;
        //public const byte BeamFormingMicrophoneHDReferenceSelectionObject_1_Min = 0;
        //public const byte BeamFormingMicrophoneHDReferenceSelectionObject_2_Default = 0;
        //public const byte BeamFormingMicrophoneHDReferenceSelectionObject_2_Increment = 1;
        //public const byte BeamFormingMicrophoneHDReferenceSelectionObject_2_Max = 255;
        //public const byte BeamFormingMicrophoneHDReferenceSelectionObject_2_Min = 0;
        //public const byte BeamFormingMicrophoneMuteCommandDeviceID_Default = 0;
        //public const byte BeamFormingMicrophoneMuteCommandDeviceID_Increment = 1;
        //public const byte BeamFormingMicrophoneMuteCommandDeviceID_Max = 16;
        //public const byte BeamFormingMicrophoneMuteCommandDeviceID_Min = 0;
        //public const byte BeamFormingMicrophoneDeviceID_Default = 0;
        //public const byte BeamFormingMicrophoneDeviceID_Increment = 1;
        //public const byte BeamFormingMicrophoneDeviceID_Max = 16;
        //public const byte BeamFormingMicrophoneDeviceID_Min = 0;
        //public const byte BeamFormingMicrophoneMuteCommandDeviceTypeID_Default = 0;
        //public const byte BeamFormingMicrophoneMuteCommandDeviceTypeID_Increment = 1;
        //public const byte BeamFormingMicrophoneMuteCommandDeviceTypeID_Max = 255;
        //public const byte BeamFormingMicrophoneMuteCommandDeviceTypeID_Min = 0;
        //public const UInt16 BeamFormingMicrophoneMuteOffCommand_Max = 5000;
        //public const UInt16 BeamFormingMicrophoneMuteOffCommand_Min = 0;
        //public const UInt16 BeamFormingMicrophoneMuteOnCommand_Max = 5000;
        //public const UInt16 BeamFormingMicrophoneMuteOnCommand_Min = 0;
        //public const byte VersionMajor_Default = 0;
        //public const byte VersionMajor_Increment = 1;
        //public const byte VersionMajor_Max = 255;
        //public const byte VersionMajor_Min = 0;
        //public const byte VersionMinor_Default = 0;
        //public const byte VersionMinor_Increment = 1;
        //public const byte VersionMinor_Max = 255;
        //public const byte VersionMinor_Min = 0;
        //public const byte VersionRevision_Default = 0;
        //public const byte VersionRevision_Increment = 1;
        //public const byte VersionRevision_Max = 255;
        //public const byte VersionRevision_Min = 0;
        //public const byte VersionBuild_Default = 0;
        //public const byte VersionBuild_Increment = 1;
        //public const byte VersionBuild_Max = 255;
        //public const byte VersionBuild_Min = 0;
        //public const byte VersionID_Default = 1;
        //public const byte VersionID_Increment = 1;
        //public const byte VersionID_Max = 255;
        //public const byte VersionID_Min = 1;
        //public const UInt16 TimedEventEndTime_Default = 0;
        //public const UInt16 TimedEventEndTime_Increment = 1;
        //public const UInt16 TimedEventEndTime_Max = 5947;
        //public const UInt16 TimedEventEndTime_Min = 0;
        //public const UInt16 TimedEventStartTime_Default = 0;
        //public const UInt16 TimedEventStartTime_Increment = 1;
        //public const UInt16 TimedEventStartTime_Max = 5947;
        //public const UInt16 TimedEventStartTime_Min = 0;
        //public const byte TimedEventNumberOccurrances_Default = 0;
        //public const byte TimedEventNumberOccurrances_Increment = 1;
        //public const byte TimedEventNumberOccurrances_Max = 255;
        //public const byte TimedEventNumberOccurrances_Min = 0;
        //public const byte TimedEventRepeatCount_Default = 0;
        //public const byte TimedEventRepeatCount_Increment = 1;
        //public const byte TimedEventRepeatCount_Max = 255;
        //public const byte TimedEventRepeatCount_Min = 0;
        //public const UInt16 TimedEventStartDate_Max = 10;
        //public const UInt16 TimedEventStartDate_Min = 1;
        //public const UInt16 TimedEventEndDate_Max = 10;
        //public const UInt16 TimedEventEndDate_Min = 1;
        //public const byte TimedEventReoccurPattern_Default = 0;
        //public const byte TimedEventReoccurPattern_Increment = 1;
        //public const byte TimedEventReoccurPattern_Max = 3;
        //public const byte TimedEventReoccurPattern_Min = 0;
        //public const byte TimedEventReoccurDays_Default = 0;
        //public const byte TimedEventReoccurDays_Increment = 1;
        //public const byte TimedEventReoccurDays_Max = 64;
        //public const byte TimedEventReoccurDays_Min = 0;
        //public const byte TimedEventID_Default = 1;
        //public const byte TimedEventID_Increment = 1;
        //public const byte TimedEventID_Max = 255;
        //public const byte TimedEventID_Min = 1;
        //public const byte DeviceTelnetEnable_Default = 0;
        //public const byte DeviceTelnetEnable_Increment = 1;
        //public const byte DeviceTelnetEnable_Max = 2;
        //public const byte DeviceTelnetEnable_Min = 0;
        //public const byte DeviceSerialEchoEnable_Default = 0;
        //public const byte DeviceSerialEchoEnable_Increment = 1;
        //public const byte DeviceSerialEchoEnable_Max = 2;
        //public const byte DeviceSerialEchoEnable_Min = 0;
        //public const Int32 DeviceTelnetPort_Default = 0;
        //public const Int32 DeviceTelnetPort_Increment = 1;
        //public const Int32 DeviceTelnetPort_Max = 65535;
        //public const Int32 DeviceTelnetPort_Min = 0;
        //public const UInt32 DeviceIPMask_Default = 0;
        //public const UInt32 DeviceIPMask_Increment = 1;
        //public const UInt32 DeviceIPMask_Max = 4294967040;
        //public const UInt32 DeviceIPMask_Min = 0;
        //public const int DeviceIPMask_Count = 8;
        //public const byte DeviceDeviceID_Default = 0;
        //public const byte DeviceDeviceID_Increment = 1;
        //public const byte DeviceDeviceID_Max = 16;
        //public const byte DeviceDeviceID_Min = 0;
        //public const UInt16 GlobalSystemTimeZoneName_Max = 64;
        //public const UInt16 GlobalSystemTimeZoneName_Min = 1;
        //public const UInt16 GlobalSystemSystemName_Max = 64;
        //public const UInt16 GlobalSystemSystemName_Min = 1;
        //public const UInt16 GlobalSystemCountryString_Max = 64;
        //public const UInt16 GlobalSystemCountryString_Min = 1;
        //public const UInt16 GlobalSystemStateString_Max = 64;
        //public const UInt16 GlobalSystemStateString_Min = 1;
        //public const UInt16 GlobalSystemCityString_Max = 64;
        //public const UInt16 GlobalSystemCityString_Min = 1;
        //public const UInt16 GlobalSystemCompanyString_Max = 64;
        //public const UInt16 GlobalSystemCompanyString_Min = 1;
        //public const UInt16 GlobalSystemBuildingString_Max = 64;
        //public const UInt16 GlobalSystemBuildingString_Min = 1;
        //public const UInt16 GlobalSystemRegionString_Max = 64;
        //public const UInt16 GlobalSystemRegionString_Min = 1;
        //public const byte GlobalSystemDaylightSavings_Default = 0;
        //public const byte GlobalSystemDaylightSavings_Increment = 1;
        //public const byte GlobalSystemDaylightSavings_Max = 2;
        //public const byte GlobalSystemDaylightSavings_Min = 0;
        //public const UInt32 GlobalSystemTimeServer1_Default = 0;
        //public const UInt32 GlobalSystemTimeServer1_Increment = 1;
        //public const UInt32 GlobalSystemTimeServer1_Max = 4294967040;
        //public const UInt32 GlobalSystemTimeServer1_Min = 0;
        //public const UInt32 GlobalSystemTimeServer2_Default = 0;
        //public const UInt32 GlobalSystemTimeServer2_Increment = 1;
        //public const UInt32 GlobalSystemTimeServer2_Max = 4294967040;
        //public const UInt32 GlobalSystemTimeServer2_Min = 0;
        //public const byte GlobalSystemID_Default = 1;
        //public const byte GlobalSystemID_Increment = 1;
        //public const byte GlobalSystemID_Max = 255;
        //public const byte GlobalSystemID_Min = 1;
        //public const UInt16 GlobalSystemCommandList_Count = 255;
        //public const UInt16 SNMPSettingsContext_Max = 32;
        //public const UInt16 SNMPSettingsContext_Min = 1;
        //public const UInt16 SNMPSettingsPrivateCommunity_Max = 32;
        //public const UInt16 SNMPSettingsPrivateCommunity_Min = 1;
        //public const UInt16 SNMPSettingsPublicCommunity_Max = 32;
        //public const UInt16 SNMPSettingsPublicCommunity_Min = 1;
        //public const UInt16 SNMPSettingsUserName_Max = 32;
        //public const UInt16 SNMPSettingsUserName_Min = 1;
        //public const byte SNMPSettingsAuthenticationEnable_Default = 0;
        //public const byte SNMPSettingsAuthenticationEnable_Increment = 1;
        //public const byte SNMPSettingsAuthenticationEnable_Max = 2;
        //public const byte SNMPSettingsAuthenticationEnable_Min = 0;
        //public const byte SNMPSettingsPrivacyEnable_Default = 0;
        //public const byte SNMPSettingsPrivacyEnable_Increment = 1;
        //public const byte SNMPSettingsPrivacyEnable_Max = 2;
        //public const byte SNMPSettingsPrivacyEnable_Min = 0;
        //public const Int32 SNMPSettingsManagerPort_Default = 0;
        //public const Int32 SNMPSettingsManagerPort_Increment = 1;
        //public const Int32 SNMPSettingsManagerPort_Max = 65535;
        //public const Int32 SNMPSettingsManagerPort_Min = 0;
        //public const Int64 SNMPSettingsManagerIP_Default = 0;
        //public const Int64 SNMPSettingsManagerIP_Increment = 1;
        //public const Int64 SNMPSettingsManagerIP_Max = 4294967040;
        //public const Int64 SNMPSettingsManagerIP_Min = 0;
        //public const byte SNMPSettingsID_Default = 1;
        //public const byte SNMPSettingsID_Increment = 1;
        //public const byte SNMPSettingsID_Max = 255;
        //public const byte SNMPSettingsID_Min = 1;
        //public const byte GateEnable_Default = 0;
        //public const byte GateEnable_Increment = 1;
        //public const byte GateEnable_Max = 2;
        //public const byte GateEnable_Min = 0;
        //public const byte GateID_Default = 1;
        //public const byte GateID_Increment = 1;
        //public const byte GateID_Max = 255;
        //public const byte GateID_Min = 1;
        //public const byte NoiseCancellationEnable_Default = 0;
        //public const byte NoiseCancellationEnable_Increment = 1;
        //public const byte NoiseCancellationEnable_Max = 2;
        //public const byte NoiseCancellationEnable_Min = 0;
        //public const byte NoiseCancellationID_Default = 1;
        //public const byte NoiseCancellationID_Increment = 1;
        //public const byte NoiseCancellationID_Max = 255;
        //public const byte NoiseCancellationID_Min = 1;
        //public const UInt32 VoipTLSZippedCertsLength_Default = 0;
        //public const UInt32 VoipTLSZippedCertsLength_Increment = 1;
        //public const UInt32 VoipTLSZippedCertsLength_Max = 20971520;
        //public const UInt32 VoipTLSZippedCertsLength_Min = 0;
        //public const UInt32 VoipTLSZippedCerts_Default = 0;
        //public const UInt32 VoipTLSZippedCerts_Increment = 1;
        //public const UInt32 VoipTLSZippedCerts_Max = 20971520;
        //public const UInt32 VoipTLSZippedCerts_Min = 0;
        //public const byte VoipTLSID_Default = 1;
        //public const byte VoipTLSID_Increment = 1;
        //public const byte VoipTLSID_Max = 255;
        //public const byte VoipTLSID_Min = 1;
        //public const byte VoipOutboundProxyEnable_Default = 0;
        //public const byte VoipOutboundProxyEnable_Increment = 1;
        //public const byte VoipOutboundProxyEnable_Max = 2;
        //public const byte VoipOutboundProxyEnable_Min = 0;
        //public const byte VoipOutboundProxyID_Default = 1;
        //public const byte VoipOutboundProxyID_Increment = 1;
        //public const byte VoipOutboundProxyID_Max = 255;
        //public const byte VoipOutboundProxyID_Min = 1;
        //public const byte PowerAmpCommonEnergySaverEnable_Default = 0;
        //public const byte PowerAmpCommonEnergySaverEnable_Increment = 1;
        //public const byte PowerAmpCommonEnergySaverEnable_Max = 2;
        //public const byte PowerAmpCommonEnergySaverEnable_Min = 0;
        //public const byte PowerAmpCommonID_Default = 1;
        //public const byte PowerAmpCommonID_Increment = 1;
        //public const byte PowerAmpCommonID_Max = 255;
        //public const byte PowerAmpCommonID_Min = 1;
        //public const byte GatingGroupFirstMicPriorityEnable_Default = 0;
        //public const byte GatingGroupFirstMicPriorityEnable_Increment = 1;
        //public const byte GatingGroupFirstMicPriorityEnable_Max = 2;
        //public const byte GatingGroupFirstMicPriorityEnable_Min = 0;
        //public const byte GatingGroupID_Default = 1;
        //public const byte GatingGroupID_Increment = 1;
        //public const byte GatingGroupID_Max = 255;
        //public const byte GatingGroupID_Min = 1;
        //public const byte VirtualReferenceID_Default = 1;
        //public const byte VirtualReferenceID_Increment = 1;
        //public const byte VirtualReferenceID_Max = 255;
        //public const byte VirtualReferenceID_Min = 1;
        //public const UInt16 GPIOHighCommand_Max = 5000;
        //public const UInt16 GPIOHighCommand_Min = 0;
        //public const UInt16 GPIOLowCommand_Max = 5000;
        //public const UInt16 GPIOLowCommand_Min = 0;
        //public const byte GPIOID_Default = 1;
        //public const byte GPIOID_Increment = 1;
        //public const byte GPIOID_Max = 255;
        //public const byte GPIOID_Min = 1;
        //public const byte MatrixCrossPointInputType_Default = 0;
        //public const byte MatrixCrossPointInputType_Increment = 1;
        //public const byte MatrixCrossPointInputType_Max = 255;
        //public const byte MatrixCrossPointInputType_Min = 0;
        //public const byte MatrixCrossPointOutputType_Default = 0;
        //public const byte MatrixCrossPointOutputType_Increment = 1;
        //public const byte MatrixCrossPointOutputType_Max = 255;
        //public const byte MatrixCrossPointOutputType_Min = 0;
        //public const byte MatrixCrossPointInputChannelID_Default = 1;
        //public const byte MatrixCrossPointInputChannelID_Increment = 1;
        //public const byte MatrixCrossPointInputChannelID_Max = 255;
        //public const byte MatrixCrossPointInputChannelID_Min = 1;
        //public const byte MatrixCrossPointOutputChannelID_Default = 1;
        //public const byte MatrixCrossPointOutputChannelID_Increment = 1;
        //public const byte MatrixCrossPointOutputChannelID_Max = 255;
        //public const byte MatrixCrossPointOutputChannelID_Min = 1;
        //public const byte MatrixCrossPointID_Default = 1;
        //public const byte MatrixCrossPointID_Increment = 1;
        //public const byte MatrixCrossPointID_Max = 255;
        //public const byte MatrixCrossPointID_Min = 1;
        //public const UInt16 MatrixColumns_Default = 1;
        //public const UInt16 MatrixColumns_Increment = 1;
        //public const UInt16 MatrixColumns_Max = 512;
        //public const UInt16 MatrixColumns_Min = 1;
        //public const UInt16 MatrixRows_Default = 1;
        //public const UInt16 MatrixRows_Increment = 1;
        //public const UInt16 MatrixRows_Max = 512;
        //public const UInt16 MatrixRows_Min = 1;
        //public const byte MatrixID_Default = 1;
        //public const byte MatrixID_Increment = 1;
        //public const byte MatrixID_Max = 255;
        //public const byte MatrixID_Min = 1;
        //public const byte CommandCommandID_Default = 0;
        //public const byte CommandCommandID_Increment = 1;
        //public const byte CommandCommandID_Max = 255;
        //public const byte CommandCommandID_Min = 0;
        //public const UInt32 CommandCommandLength_Default = 0;
        //public const UInt32 CommandCommandLength_Increment = 1;
        //public const UInt32 CommandCommandLength_Max = 1048576;
        //public const UInt32 CommandCommandLength_Min = 0;
        //public const UInt16 AVBListeners_Count = 4;
        //public const UInt16 AVBTalkers_Count = 4;
        //public const UInt16 CobraNetContact_Max = 60;
        //public const UInt16 CobraNetContact_Min = 1;
        //public const UInt16 CobraNetLocation_Max = 60;
        //public const UInt16 CobraNetLocation_Min = 1;
        //public const UInt16 CobraNetRxBundleNumber_Default = 0;
        //public const UInt16 CobraNetRxBundleNumber_Increment = 1;
        //public const UInt16 CobraNetRxBundleNumber_Max = 65535;
        //public const UInt16 CobraNetRxBundleNumber_Min = 0;
        //public const int CobraNetRxBundleNumber_Count = 4;
        //public const byte CobraNetRxBundleType_Default = 0;
        //public const byte CobraNetRxBundleType_Increment = 1;
        //public const byte CobraNetRxBundleType_Max = 1;
        //public const byte CobraNetRxBundleType_Min = 0;
        //public const int CobraNetRxBundleType_Count = 4;
        //public const UInt16 CobraNetTxBundleNumber_Default = 0;
        //public const UInt16 CobraNetTxBundleNumber_Increment = 1;
        //public const UInt16 CobraNetTxBundleNumber_Max = 65535;
        //public const UInt16 CobraNetTxBundleNumber_Min = 0;
        //public const int CobraNetTxBundleNumber_Count = 4;
        //public const byte CobraNetTxBundleType_Default = 0;
        //public const byte CobraNetTxBundleType_Increment = 1;
        //public const byte CobraNetTxBundleType_Max = 1;
        //public const byte CobraNetTxBundleType_Min = 0;
        //public const int CobraNetTxBundleType_Count = 4;
        //public const UInt64 CobraNetRxMacAddress_Default = 0;
        //public const UInt64 CobraNetRxMacAddress_Increment = 1;
        //public const UInt64 CobraNetRxMacAddress_Max = 281475010265088;
        //public const UInt64 CobraNetRxMacAddress_Min = 0;
        //public const UInt64 CobraNetTxMacAddress_Default = 0;
        //public const UInt64 CobraNetTxMacAddress_Increment = 1;
        //public const UInt64 CobraNetTxMacAddress_Max = 281475010265088;
        //public const UInt64 CobraNetTxMacAddress_Min = 0;
        //public const byte CobraNetSerialEnable_Default = 0;
        //public const byte CobraNetSerialEnable_Increment = 1;
        //public const byte CobraNetSerialEnable_Max = 2;
        //public const byte CobraNetSerialEnable_Min = 0;
        //public const byte AVBStreamNumberChannels_Default = 0;
        //public const byte AVBStreamNumberChannels_Increment = 1;
        //public const byte AVBStreamNumberChannels_Max = 4;
        //public const byte AVBStreamNumberChannels_Min = 0;
        //public const byte AVBStreamID_Default = 1;
        //public const byte AVBStreamID_Increment = 1;
        //public const byte AVBStreamID_Max = 255;
        //public const byte AVBStreamID_Min = 1;
        //public const byte VTableCount_Default = 1;
        //public const byte VTableCount_Increment = 1;
        //public const byte VTableCount_Max = 255;
        //public const byte VTableCount_Min = 1;
        //public const UInt16 VTableAuthenticationPort_Default = 0;
        //public const UInt16 VTableAuthenticationPort_Increment = 1;
        //public const UInt16 VTableAuthenticationPort_Max = 65535;
        //public const UInt16 VTableAuthenticationPort_Min = 0;
        //public const byte VTableDeviceGLinkOrder_Default = 1;
        //public const byte VTableDeviceGLinkOrder_Increment = 1;
        //public const byte VTableDeviceGLinkOrder_Max = 255;
        //public const byte VTableDeviceGLinkOrder_Min = 1;
        //public const byte VTableDeviceDeviceType_Default = 0;
        //public const byte VTableDeviceDeviceType_Increment = 1;
        //public const byte VTableDeviceDeviceType_Max = 255;
        //public const byte VTableDeviceDeviceType_Min = 0;
        //public const byte VTableDeviceNumberBeamFormers_Default = 0;
        //public const byte VTableDeviceNumberBeamFormers_Increment = 1;
        //public const byte VTableDeviceNumberBeamFormers_Max = 6;
        //public const byte VTableDeviceNumberBeamFormers_Min = 0;
        //public const byte VTableDeviceNumberCobraNets_Default = 0;
        //public const byte VTableDeviceNumberCobraNets_Increment = 1;
        //public const byte VTableDeviceNumberCobraNets_Max = 6;
        //public const byte VTableDeviceNumberCobraNets_Min = 0;
        //public const byte VTableDeviceNumberAVBs_Default = 0;
        //public const byte VTableDeviceNumberAVBs_Increment = 1;
        //public const byte VTableDeviceNumberAVBs_Max = 6;
        //public const byte VTableDeviceNumberAVBs_Min = 0;
        //public const byte PhoneBookEntrySpeedDial_Default = 0;
        //public const byte PhoneBookEntrySpeedDial_Increment = 1;
        //public const byte PhoneBookEntrySpeedDial_Max = 255;
        //public const byte PhoneBookEntrySpeedDial_Min = 0;
        //public const byte PhoneBookEntryID_Default = 0;
        //public const byte PhoneBookEntryID_Increment = 1;
        //public const byte PhoneBookEntryID_Max = 255;
        //public const byte PhoneBookEntryID_Min = 0;
        //public const UInt16 PhoneBookPhoneBookEntry_Count = 50;
        //public const byte LocationIDCount_Default = 1;
        //public const byte LocationIDCount_Increment = 1;
        //public const byte LocationIDCount_Max = 10;
        //public const byte LocationIDCount_Min = 1;
        //public const byte LocationIDVersionNumber_Default = 1;
        //public const byte LocationIDVersionNumber_Increment = 1;
        //public const byte LocationIDVersionNumber_Max = 255;
        //public const byte LocationIDVersionNumber_Min = 0;
        //public const UInt16 ConnectionUserName_Max = 256;
        //public const UInt16 ConnectionUserName_Min = 1;
        //public const UInt16 ConnectionPassword_Max = 256;
        //public const UInt16 ConnectionPassword_Min = 1;
        //public const UInt16 ConnectionCommandPort_Default = 0;
        //public const UInt16 ConnectionCommandPort_Increment = 1;
        //public const UInt16 ConnectionCommandPort_Max = 65535;
        //public const UInt16 ConnectionCommandPort_Min = 0;
        //public const UInt16 ConnectionMeterPort_Default = 0;
        //public const UInt16 ConnectionMeterPort_Increment = 1;
        //public const UInt16 ConnectionMeterPort_Max = 65535;
        //public const UInt16 ConnectionMeterPort_Min = 0;
        //public const UInt16 ConnectionFirmwarePort_Default = 0;
        //public const UInt16 ConnectionFirmwarePort_Increment = 1;
        //public const UInt16 ConnectionFirmwarePort_Max = 65535;
        //public const UInt16 ConnectionFirmwarePort_Min = 0;
        //public const UInt16 TimedEventsTimedEvent_Count = 20;
        //public const UInt16 Timpanogos8x8Input_Count = 4;
        //public const UInt16 Timpanogos8x8Microphone_Count = 8;
        //public const UInt16 Timpanogos8x8Output_Count = 12;
        //public const UInt16 Timpanogos8x8Submix_Count = 8;
        //public const UInt16 Timpanogos8x8Fader_Count = 4;
        //public const UInt16 Timpanogos8x8GLink_Count = 160;
        //public const UInt16 Timpanogos8x8GatingGroup_Count = 10;
        //public const UInt16 Timpanogos8x8VirtualReference_Count = 4;
        //public const UInt16 Timpanogos8x8GPIO_Count = 25;
        //public const UInt16 TimpanogosVoipInput_Count = 2;
        //public const UInt16 TimpanogosVoipOutput_Count = 2;
        //public const UInt16 TimpanogosVoipTimedEvent_Count = 10;
        //public const UInt16 TimpanogosVoipGLink_Count = 160;
        //public const UInt16 TimpanogosVoipGPIO_Count = 25;
        //public const UInt16 Timpanogos8x8AmpInput_Count = 4;
        //public const UInt16 Timpanogos8x8AmpMicrophone_Count = 8;
        //public const UInt16 Timpanogos8x8AmpOutput_Count = 12;
        //public const UInt16 Timpanogos8x8AmpSubmix_Count = 8;
        //public const UInt16 Timpanogos8x8AmpPowerAmp_Count = 4;
        //public const UInt16 Timpanogos8x8AmpTimedEvent_Count = 10;
        //public const UInt16 Timpanogos8x8AmpGLink_Count = 160;
        //public const UInt16 Timpanogos8x8AmpGatingGroup_Count = 10;
        //public const UInt16 Timpanogos8x8AmpVirtualReference_Count = 4;
        //public const UInt16 Timpanogos8x8AmpGPIO_Count = 25;
        //public const UInt16 Timpanogos8x8PlusInput_Count = 4;
        //public const UInt16 Timpanogos8x8PlusMicrophone_Count = 8;
        //public const UInt16 Timpanogos8x8PlusOutput_Count = 12;
        //public const UInt16 Timpanogos8x8PlusUsbTransmit_Count = 2;
        //public const UInt16 Timpanogos8x8PlusSubmix_Count = 8;
        //public const UInt16 Timpanogos8x8PlusFader_Count = 4;
        //public const UInt16 Timpanogos8x8PlusUsbReceive_Count = 2;
        //public const UInt16 Timpanogos8x8PlusTimedEvent_Count = 10;
        //public const UInt16 Timpanogos8x8PlusGLink_Count = 160;
        //public const UInt16 Timpanogos8x8PlusGatingGroup_Count = 10;
        //public const UInt16 Timpanogos8x8PlusVirtualReference_Count = 4;
        //public const UInt16 Timpanogos8x8PlusGPIO_Count = 25;
        //public const UInt16 SubroomTopX_Default = 1;
        //public const UInt16 SubroomTopX_Increment = 1;
        //public const UInt16 SubroomTopX_Max = 65535;
        //public const UInt16 SubroomTopX_Min = 1;
        //public const UInt16 SubroomBottomX_Default = 1;
        //public const UInt16 SubroomBottomX_Increment = 1;
        //public const UInt16 SubroomBottomX_Max = 65535;
        //public const UInt16 SubroomBottomX_Min = 1;
        //public const UInt16 SubroomTopY_Default = 1;
        //public const UInt16 SubroomTopY_Increment = 1;
        //public const UInt16 SubroomTopY_Max = 65535;
        //public const UInt16 SubroomTopY_Min = 1;
        //public const UInt16 SubroomBottomY_Default = 1;
        //public const UInt16 SubroomBottomY_Increment = 1;
        //public const UInt16 SubroomBottomY_Max = 65535;
        //public const UInt16 SubroomBottomY_Min = 1;
        //public const UInt32 FirmwareUpdateFirmware_Default = 0;
        //public const UInt32 FirmwareUpdateFirmware_Increment = 1;
        //public const UInt32 FirmwareUpdateFirmware_Max = 20971520;
        //public const UInt32 FirmwareUpdateFirmware_Min = 0;
        //public const UInt32 FirmwareUpdateFirmwareLength_Default = 0;
        //public const UInt32 FirmwareUpdateFirmwareLength_Increment = 1;
        //public const UInt32 FirmwareUpdateFirmwareLength_Max = 20971520;
        //public const UInt32 FirmwareUpdateFirmwareLength_Min = 0;
        //public const byte FirmwareUpgradeStatusPercentComplete_Default = 0;
        //public const byte FirmwareUpgradeStatusPercentComplete_Increment = 1;
        //public const byte FirmwareUpgradeStatusPercentComplete_Max = 100;
        //public const byte FirmwareUpgradeStatusPercentComplete_Min = 0;
        //public const byte MeterQueryCount_Default = 1;
        //public const byte MeterQueryCount_Increment = 1;
        //public const byte MeterQueryCount_Max = 255;
        //public const byte MeterQueryCount_Min = 1;
        //public const byte MeterResponseCount_Default = 1;
        //public const byte MeterResponseCount_Increment = 1;
        //public const byte MeterResponseCount_Max = 255;
        //public const byte MeterResponseCount_Min = 1;
        //public const UInt32 GetValueResponseValueArrayLength_Default = 0;
        //public const UInt32 GetValueResponseValueArrayLength_Increment = 1;
        //public const UInt32 GetValueResponseValueArrayLength_Max = 20971520;
        //public const UInt32 GetValueResponseValueArrayLength_Min = 0;
        //public const UInt32 GetValueResponseValueArray_Default = 0;
        //public const UInt32 GetValueResponseValueArray_Increment = 1;
        //public const UInt32 GetValueResponseValueArray_Max = 20971520;
        //public const UInt32 GetValueResponseValueArray_Min = 0;
        //public const UInt32 SetValueValueArrayLength_Default = 0;
        //public const UInt32 SetValueValueArrayLength_Increment = 1;
        //public const UInt32 SetValueValueArrayLength_Max = 20971520;
        //public const UInt32 SetValueValueArrayLength_Min = 0;
        //public const UInt32 SetValueValueArray_Default = 0;
        //public const UInt32 SetValueValueArray_Increment = 1;
        //public const UInt32 SetValueValueArray_Max = 20971520;
        //public const UInt32 SetValueValueArray_Min = 0;
	}
}
