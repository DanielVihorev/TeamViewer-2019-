using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Threading.Tasks;

public class PacketWriter : BinaryWriter
{
    private MemoryStream _ms;
    private BinaryFormatter _bf;

    public PacketWriter()
        : base()
    {
        _ms = new MemoryStream();
        _bf = new BinaryFormatter();
        OutStream = _ms;
    }

    public void Write(Image image)
    {
        var ms = new MemoryStream();
        image.Save(ms, ImageFormat.Png);

        ms.Close();

        byte[] imageBytes = ms.ToArray();

        Write(imageBytes.Length);
        Write(imageBytes);
    }

    public void WriteT(object obj)
    {
        // need to decrypt here

        //using (Aes aes = Aes.Create())
        //{
        //    byte[] originalData = _ms.GetBuffer();
        //    byte[] decryptedData = Decrypt(originalData, aes.Key, aes.IV);
        //    _ms.Flush();
        //    _ms.Write(decryptedData, 0, decryptedData.Length);
        //}

        _bf.Serialize(_ms, obj);
    }

    public byte[] GetBytes()
    {
        Close();

        byte[] data = _ms.ToArray();

        return data;
    }


    //public byte[] Decrypt(byte[] bytes, byte[] key, byte[] IV)
    //{
    //    if (bytes == null || key == null || IV == null || bytes.Length <= 0 || key.Length <= 0 || IV.Length <= 0)
    //    {
    //        return null;
    //    }
    //    byte[] data;
    //    // create aes object with key and IV
    //    using (Aes aes = Aes.Create())
    //    {
    //        aes.Key = key;
    //        aes.IV = IV;
    //        using (MemoryStream ms = new MemoryStream(bytes))
    //        {
    //            using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(aes.Key, aes.IV), CryptoStreamMode.Read))
    //            {
    //                using (StreamReader sr = new StreamReader(cs))
    //                {
    //                    // Write the data to the stream
    //                    data = Encoding.ASCII.GetBytes(sr.ReadToEnd());
    //                }

    //                data = ms.ToArray();
    //            }
    //        }
    //    }
    //    return data;
    //}
}

public class PacketReader : BinaryReader
{
    private BinaryFormatter _bf;
    public PacketReader(byte[] data)
        : base(new MemoryStream(data))
    {
        _bf = new BinaryFormatter();
    }

    public Image ReadImage()
    {
        int len = ReadInt32();

        byte[] bytes = ReadBytes(len);

        Image img;

        using (MemoryStream ms = new MemoryStream(bytes))
        {
            img = Image.FromStream(ms);
        }

        return img;
    }

    public T ReadObject<T>()
    {
        // need to eycrypt here
        //using (Aes aes = Aes.Create())
        //{
        //    byte[] originalData = new byte[int.MaxValue];
        //    BaseStream.Read(originalData, 0, originalData.Length);
        //    byte[] encryptedData = Encrypt(originalData, aes.Key, aes.IV);
        //    BaseStream.Flush();
        //    BaseStream.Write(encryptedData, 0, encryptedData.Length);
        //}

        return (T)_bf.Deserialize(BaseStream);
    }



    //public byte[] Encrypt(byte[] bytes, byte[] key, byte[] IV)
    //{
    //    if (bytes == null || key == null || IV == null || bytes.Length <= 0 || key.Length <= 0 || IV.Length <= 0)
    //    {
    //        return null;
    //    }
    //    byte[] data;
    //    // create aes object with key and IV
    //    using (Aes aes = Aes.Create())
    //    {
    //        aes.Key = key;
    //        aes.IV = IV;
    //        using (MemoryStream ms = new MemoryStream())
    //        {
    //            using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(aes.Key, aes.IV), CryptoStreamMode.Write))
    //            {
    //                using (StreamWriter sw = new StreamWriter(cs))
    //                {
    //                    // Write the data to the stream
    //                    sw.Write(bytes);
    //                }

    //                data = ms.ToArray();
    //            }
    //        }
    //    }
    //    return data;
    //}

}


