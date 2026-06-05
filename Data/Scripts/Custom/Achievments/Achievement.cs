using Server;
using System;


namespace Scripts.Mythik.Systems.Achievements
{
    class AchieveData
    {
        //public int ID { get; set; }
        public int Progress { get; set; }
        public DateTime CompletedOn { get; set; }
        public bool IsComplete { get { return CompletedOn != DateTime.MinValue; } }

        public AchieveData()
        {
        }

        public AchieveData(GenericReader reader)
        {
            Deserialize(reader);
        }
        public void Serialize(GenericWriter writer)
        {
            writer.Write(1); // version
            writer.Write(Progress);
            writer.Write(CompletedOn);

        }

        public void Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();
            Progress = reader.ReadInt();
            CompletedOn = reader.ReadDateTime();
        }
    }
}
