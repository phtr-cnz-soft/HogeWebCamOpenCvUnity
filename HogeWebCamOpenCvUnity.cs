using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;

public class HogeWebCamOpenCvUnity : MonoBehaviour
{

    //画像処理後の映像出力先
    public RawImage HogeOutputRaw;

    //各々のウェブカメラの設定用項目
    [SerializeField] private int width = 1920;
    [SerializeField] private int height = 1080;
    [SerializeField] private int fps = 30;

    //ウェブカメラテクスチャ
    private WebCamTexture hogeWebCamTexture;

    void Start()
    {
        //ウェブカメラの初期設定と起動
        hogeWebCamTexture = new WebCamTexture(width, height, fps);
        HogeOutputRaw.texture = hogeWebCamTexture;
        hogeWebCamTexture.Play();
    }

    void Update()
    {
        //ウェブカメラのフレームが変更されたら処理
        if (hogeWebCamTexture.didUpdateThisFrame)
        {
            //ウェブカメラの画像をMatに変換
            Mat originMat = new Mat(hogeWebCamTexture.height, hogeWebCamTexture.width, CvType.CV_8UC4);
            Utils.webCamTextureToMat(hogeWebCamTexture, originMat);
            
            //画像処理先Mat
            Mat changeMat = new Mat(originMat.cols(), originMat.rows(), CvType.CV_8UC4);

            //グレースケール処理
            Imgproc.cvtColor(originMat, changeMat, Imgproc.COLOR_RGB2GRAY);
            //二値化(数値は各々で変更)
            Imgproc.threshold(changeMat, changeMat, 100, 255, Imgproc.THRESH_BINARY);
            //輪郭抽出
            Imgproc.Sobel(changeMat, changeMat, -1, 1, 0);

            //Matをテクスチャに変換
            Texture2D endTexture = new Texture2D(changeMat.cols(), changeMat.rows(), TextureFormat.RGBA32, false);
            Utils.matToTexture2D(changeMat, endTexture);

            //映像出力先にテクスチャ貼付け
            HogeOutputRaw.texture = endTexture;
        }
    }

}
