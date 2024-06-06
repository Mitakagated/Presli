using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lavalink4NET.Tracks;

namespace Presli.Classes;
internal class SongsQueue
{
    internal Queue<LavalinkTrack> songQueue = new Queue<LavalinkTrack>();
    internal bool loopEnabled = false;
    private static readonly SongsQueue instance = new SongsQueue();
    static SongsQueue()
    {

    }
    private SongsQueue()
    {

    }
    public static SongsQueue Instance
    {
        get
        {
            return instance;
        }
    }

    internal void EnqueueSong(LavalinkTrack song)
    {
        songQueue.Enqueue(song);
    }

    internal void EnqueueSong(IEnumerable<LavalinkTrack> songs)
    {
        foreach (var song in songs)
        {
            songQueue.Enqueue(song);
        }
    }

    internal LavalinkTrack DequeueSong()
    {
        if (songQueue.Count > 0)
        {
            return songQueue.Dequeue();
        }
        else
        {
            return null;
        }
    }

    internal void ToggleLoop()
    {
        loopEnabled = !loopEnabled;
    }

    internal LavalinkTrack PlaySong()
    {
        if (songQueue.Count > 0)
        {
            if (loopEnabled)
            {
                return songQueue.Peek();
            }
            else
            {
                return DequeueSong();
            }
        }
        else
        {
            return null;
        }
    }
}