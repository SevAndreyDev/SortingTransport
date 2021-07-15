namespace EnglishKids.SortingTransport
{
    public enum Orientations
    {
        Left,
        Right
    }

    public enum ColorKinds
    {
        Yellow,
        Green,
        Black,
        Blue,
        Brown,
        Gray,
        Orange,
        Pink,
        Purple,
        Red,
        White
    }

    public enum TransportKinds
    {
        Bike,
        Car,
        Tractor,
        Helicopter
    }

    public enum PoolObjectKinds
    {
        ConveyerItem,
        Star,
        StarEffect,
        Cloud
    }

    public enum GameEvents
    {
        RefreshSpeachButton,
        PrepareToResetGame,
        ResetGameSceneObjects,
        Action
    }

    public class GameConstants
    {
        public const float HALF_FACTOR = 0.5f;
    }
}