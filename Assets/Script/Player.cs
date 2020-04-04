using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : CharacterBase
{
    //インスペクターから設定するもの---------
    //ジャンプ力
    [SerializeField] float JumpForce = default;
    //ジャンプ継続最小時間（ジャンプしたら最低でもここまでは跳び続ける）
    [SerializeField] float MinJumpTime = default;
    //ジャンプ継続最大時間(ジャンプし続けてもこれ以上は跳べない)
    [SerializeField] float MaxJumpTime = default;
    //ジャンプ入力終了後に浮遊する時間
    [SerializeField] float FloatingTime = default;
    
    //ジャンプ関連--------------
    //ジャンプしているか
    public bool Jumping = false;
    //ジャンプ中の時間のカウント
    float jumptimer = 0;
    //浮遊中か
    bool floating = false;

    //コンポーネント------------
    //その他の変数-----------------

    void Start()
    {
        base.Initialize();
    }
    //入力関連まとめ
    void InputManager()
    {

        //X方向に入力された値を保存
        float HorizontalKey = Input.GetAxis("Horizontal");
        //横移動速度
        float speedX = 0.0f;

        //右方向に入力されていたら
        if (HorizontalKey > 0)
        {
            //右方向に速度代入
            speedX =base.MoveSpeed;
        }
        //左方向に入力されていたら
        else if (HorizontalKey < 0)
        {
            //左方向に速度代入
            speedX = -base.MoveSpeed;
        }
        else
        {
            //移動入力していない
            speedX = 0.0f;
        }

        //縦移動速度
        float speedY = 0.0f;
        //接地していてジャンプ入力がされた、又はジャンプ中で最大までジャンプしてなかったら
        if ((base.OnGround && Input.GetButton("Jump")) || Jumping)
        {
            //接地していてジャンプ入力がされた場合はジャンプ中のフラグをオンに
            if ((base.OnGround && Input.GetButton("Jump")))
            {
                //ジャンプフラグをオンに
                Jumping = true;
                //接地フラグをオフに
                base.OnGround = false;
                Debug.Log("ジャンプ開始");

            }

            //ジャンプの最低空までは跳び続ける
            if (jumptimer < MinJumpTime)
            {
                Debug.Log("最低空までは跳び続ける");
                speedY = JumpForce; //ジャンプ力を代入
            }
            else if (Input.GetButton("Jump") && (jumptimer < MaxJumpTime))
            {
                //任意のジャンプ継続
                speedY = JumpForce; //ジャンプ力を代入
            }
            else
            {
                //ジャンプ入力が終了
                //ジャンプタイマーを上限にする
                jumptimer = MaxJumpTime;
                //ジャンプ入力終了
                floating = true;
                Jumping = false;     
                Debug.Log("着地までお待ちください");
            }

            Debug.Log(jumptimer);
            //ジャンプが終わっていなければジャンプ入力継続時間をプラス
            if (!floating)
                jumptimer += Time.deltaTime;
        }
        //ジャンプ入力が終了
        if (floating)
        {
            Debug.Log("超越神力");
            //ちょっと浮く
            rb.gravityScale = 0;

            //浮遊継続時間が最大なら
            if (jumptimer >= (MaxJumpTime + FloatingTime))
            {
                //浮遊終了
                floating = false;

                //重力を元通りに
                rb.gravityScale = Gravity;
            }
            //ジャンプ入力継続時間をプラス
            jumptimer += Time.deltaTime;
        }
        //速度適用
        rb.velocity = new Vector2(speedX, speedY);
    }

    //接地しているかチェック
    protected override void OnGroundCheck()
    {
        //レイを飛ばして接地をチェック
      
        int layerMask = 1;//レイを飛ばすレイヤー

        //キャラクターのレイヤーを一時的に変更
        LayerMask layerM = this.gameObject.layer;//現在のレイヤーを保存
        this.gameObject.layer = 2;//キャラクターをレイの当たらないレイヤーに変更

        //レイを飛ばして当たったオブジェクトの情報を格納
        RaycastHit2D raycast = Physics2D.Raycast(transform.position, Vector2.down, GroundCheckRayPos, layerMask);

        //キャラクターのレイヤーを戻す
        this.gameObject.layer = layerM;
        /*レイを描画
        Debug.DrawRay(transform.position, Vector2.down * GroundCheckRayPos, Color.green);
        */
        
        if (raycast.transform.gameObject == null) return;
        //当たったオブジェクトが地面なら
        if ( raycast.collider.gameObject.tag == "Ground" )
        {
            OnGround = true;//接地フラグをオンに
            Jumping = false;//ジャンプ中のフラグをリセット            
            jumptimer = 0;  //ジャンプ入力継続時間をリセット
            floating = false; //浮遊のフラグをリセット
            rb.gravityScale = Gravity;//重力を元通りに
        }
    }

    void FixedUpdate()
    {
        //入力処理
        InputManager();
    }


    //当たり判定
    void OnCollisionEnter2D(Collision2D col)
    {
        //地面に当たっていて、接地していないかったら
        if (col.gameObject.tag == "Ground" && !OnGround)
        {
            //接地するかどうかチェック
           OnGroundCheck();

        }
    }
}
