using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionDestination : MonoBehaviour
{
    //此传送门为哪点
    public enum DestinationTag
    {
        ENTER,A,B,C,D
    }
    public DestinationTag destinationTag;
}
