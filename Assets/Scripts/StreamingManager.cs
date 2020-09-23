using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.WebRTC;
using System;
using System.Numerics;
using Unity.RenderStreaming;

public interface IConnectionTracker
{
    int OnConnect();
    void OnDisconnect(int cid);

    int GetConnectionIDForTrack(MediaStreamTrack track);
    List<MediaStreamTrack> GetTracksForConnectionID(int cid);
}

public class StreamingManager : MonoBehaviour, IConnectionTracker
{
    public static StreamingManager Instance { get; private set; }

    private Dictionary<int, List<MediaStreamTrack>> connections = new Dictionary<int, List<MediaStreamTrack>>();
    private Dictionary<int, GameObject> cameras = new Dictionary<int, GameObject>();
    private Dictionary<int, DefaultInput> remoteInputs = new Dictionary<int, DefaultInput>();
    
    [SerializeField]
    public GameObject playerPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public int OnConnect()
    {
        int newConnectionID = 1;
        List<MediaStreamTrack> newTracks = new List<MediaStreamTrack>();

        //create camera prefab for new connection
        var newPlayer = Instantiate(playerPrefab, UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity);
        
        //route the controller for the camera
        var newInput = new DefaultInput();
        var cameraController = newPlayer.GetComponent<SimpleCameraController>();
        cameraController.SetInput(newInput);
        remoteInputs.Add(newConnectionID, newInput);
        
        //create track from camera and listener

        //add tracks to newTracks

        connections.Add(newConnectionID, newTracks);
        return newConnectionID;
    }

    public void OnDisconnect(int cid)
    {
       //destroy camera prefab
       if (connections.ContainsKey(cid))
        {
            foreach (var track in connections[cid])
            {
                track.Stop();
                track.Dispose();
            }
            connections.Remove(cid);
        }
    }
    public int GetConnectionIDForTrack(MediaStreamTrack track)
    {
        foreach (var pair in connections)
        {
            if(pair.Value.Contains(track))
            {
                return pair.Key;
            }
        }

        return -1;
    }

    public List<MediaStreamTrack> GetTracksForConnectionID(int cid)
    {
        List<MediaStreamTrack> tracks = null;

        if (connections.ContainsKey(cid))
        {
            tracks = connections[cid];
        }

        return tracks;
    }
}
