using System.Collections;
using UnityEngine;

public static class Utility{

    public static T[] ShuffleArray<T> (T[] array, int seed)
    {
        System.Random prng = new System.Random(seed);
        for(int i = 0; i < array.Length-1; i++)
        {
            int randomIndex = prng.Next(i, array.Length);
            T tempItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempItem;
        }
        return array;
    } 
        
    public static AudioClip randomlySelectAudioClipAndPitch(AudioClip[] arr, float pitchRange, out float pitchmod) {
        int selectedsound = -1;
        pitchmod = ((float) (Random.Range (0,11)*(pitchRange * 2))) + (-pitchRange);//tweak pitch from range (0.05 to -0.05)
        if (arr != null && arr.Length > 0) {
            selectedsound = Random.Range (0, arr.Length);
        }
        return arr [selectedsound];
    }
}
