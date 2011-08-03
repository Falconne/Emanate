namespace Emanate.Core.Output.DelcomVdi
{
    public class Color
    {
        private Color(int setId, int dutyId, int offsetId, int powerId)
        {
            SetId = (byte)setId;
            DutyId = (byte)dutyId;
            OffsetId = (byte)offsetId;
            PowerId = (byte)powerId;
        }

        public byte SetId { get; private set; }
        public byte DutyId { get; private set; }
        public byte OffsetId { get; private set; }
        public byte PowerId { get; private set; }

        public static Color Green = new Color(1, 21, 26, 0);
        public static Color Yellow = new Color(4, 23, 28, 2);
        public static Color Red = new Color(2, 22, 27, 1);
    }
}