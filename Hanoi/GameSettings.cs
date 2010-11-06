using ProtoBuf;

namespace Hanoi
{
    [ProtoContract]
    public class GameSettings
    {
        [ProtoMember(1)]
        public bool? playSounds;

        [ProtoMember(2)]
        public double? soundVolume;

        [ProtoMember(3)]
        public bool? showTitleBar;

        [ProtoMember(4)]
        public bool? showMoveCounter;

        [ProtoMember(5)]
        public bool? showTimer;

        [ProtoMember(6)]
        public bool? showLevel;

        [ProtoMember(7)]
        public bool? vibrateOnInvalidMove;


        public bool PlaySounds
        {
            get
            {
                if (playSounds == null)
                    return true;

                return playSounds.Value;
            }
            set
            {
                playSounds = value;
            }
        }


        public double SoundVolume
        {
            get
            {
                if (soundVolume == null)
                    return 100;

                return soundVolume.Value;
            }
            set
            {
                soundVolume = value;
            }

        }


        public bool ShowTitleBar
        {
            get
            {
                if (showTitleBar == null)
                    return true;

                return showTitleBar.Value;
            }
            set
            {
                showTitleBar = value;
            }
        }


        public bool ShowMoveCounter
        {
            get
            {
                if (showMoveCounter == null)
                    return true;

                return showMoveCounter.Value;
            }
            set
            {
                showMoveCounter = value;
            }
        }



        public bool ShowTimer
        {
            get
            {
                if (showTimer == null)
                    return true;

                return showTimer.Value;
            }
            set
            {
                showTimer = value;
            }
        }


        public bool ShowLevel
        {
            get
            {
                if (showLevel == null)
                    showLevel = true;

                return showLevel.Value;
            }
            set
            {
                showLevel = value;
            }
        }


        public bool VibrateOnInvalidMove
        {
            get
            {
                if (vibrateOnInvalidMove == null)
                    return true;

                return vibrateOnInvalidMove.Value;
            }
            set
            {
                vibrateOnInvalidMove = value;
            }
        }
    }
}
