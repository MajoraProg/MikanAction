using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    //インスペクターから設定するもの---------
    //重力
    [SerializeField] protected float Gravity = default;
    //移動の速さ
    [SerializeField] protected float MoveSpeed = default;
    //接地判定のレイをどこまで飛ばすか
    [SerializeField] protected float GroundCheckRayPos = default;

    //コンポーネント------------
    //リジッドボディ    
    protected Rigidbody2D rb = null;

    //ジャンプ関連--------------
    //接地しているか
    protected bool OnGround = false;

    //初期化
    protected void Initialize()
    {
        //コンポーネントの取得
        rb = GetComponent<Rigidbody2D>();
        //重力を設定
        rb.gravityScale = Gravity;
    }


    //接地しているかチェック
    protected virtual void OnGroundCheck()
    {
        //レイを飛ばして接地をチェック

        //レイを飛ばすレイヤー
        int layerMask = 1;

        //キャラクターのレイヤーを一時的に変更
        LayerMask layerM = this.gameObject.layer;//現在のレイヤーを保存
        this.gameObject.layer = 2;//キャラクターをレイの当たらないレイヤーに変更

        //レイを飛ばして当たったオブジェクトの情報を格納
        RaycastHit2D raycast = Physics2D.Raycast(transform.position, Vector2.down, GroundCheckRayPos, layerMask);


        this.gameObject.layer = layerM; ;//キャラクターのレイヤーを戻す

        //当たったオブジェクトが地面なら
        if (raycast.collider.gameObject.tag == "Ground")
        {
            OnGround = true;//接地フラグをオンに
        }
    }
  
}

