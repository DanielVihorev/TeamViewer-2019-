using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// here are some headers for every file that we want to transfer
namespace client_ppp
{
    public enum Headers : byte
    {
        Queue,
        Start,
        Stop,
        Pause,
        Chunk
    }
}
