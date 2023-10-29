using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource[] audioSources;

    /// <summary>
    /// BGM再生
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>

    public IEnumerator PlayBGM(int index)
    {
        // 再生前に別の曲が流れている場合
        if (index != 0)
        {
            // 徐々にボリュームを下げる
            audioSources[index - 1].DOFade(0, 0.75f);
            // Debug.Log("前の曲のボリュームを下げる");
        }
        if (index == 3)
        {
            // 徐々にボリュームを下げる
            audioSources[index - 2].DOFade(0, 0.75f);
        }


        // 前の曲のボリュームが下がるのを待つ
        yield return new WaitForSeconds(0.45f);

        // 新しい指定された曲を再生
        audioSources[index].Play();

        // Debug.Log("新しい曲を再生し、ボリュームを上げる")

        // 徐々にボリュームを上げていくことで、前の曲と重なるクロスフェード演出ができる
        audioSources[index].DOFade(0.1f, 0.75f);        // 今後追加するSEが聞こえるようにBGMは小さめのバランスにする

        // 前に流れていたBGMを停止
        if (index != 0)
        {
            // 前に流れていた曲のボリュームが0になったら
            yield return new WaitUntil(() => audioSources[index - 1].volume == 0);

            // 再生を停止
            audioSources[index - 1].Stop();
            // Debug.Log("前の曲を停止");
        }
    }
}
