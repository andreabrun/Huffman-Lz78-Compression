public interface Serializable
{
    byte[] Serialize();

    static Serializable Deserialize()
    {
        return null;
    }
}