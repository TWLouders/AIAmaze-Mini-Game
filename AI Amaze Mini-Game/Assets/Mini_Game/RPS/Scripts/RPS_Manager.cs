using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public enum HandSign
{
    None,
    Rock,
    Paper,
    Scissors
}

public class RPS_Manager : MonoBehaviour
{
    [Header("UI 文字顯示")]
    [SerializeField] private TextMeshProUGUI Text_Info;
    
    [Header("選擇顯示")]
    [SerializeField] private Image Image_PlayerChoice;
    [SerializeField] private Image Image_ComputerChoice;
    
    [Header("玩家選擇按鈕")]
    [SerializeField] private Button Button_Rock;
    [SerializeField] private Button Button_Paper;
    [SerializeField] private Button Button_Scissors;
    
    [Header("重玩按鈕")]
    [SerializeField] private Button Button_Restart;
    
    [Header("選擇圖示 (可選)")]
    [SerializeField] private Sprite Sprite_Rock;
    [SerializeField] private Sprite Sprite_Paper;
    [SerializeField] private Sprite Sprite_Scissors;
    [SerializeField] private Sprite Sprite_QuestionMark;
    
    private HandSign playerSign = HandSign.None;
    private HandSign computerSign = HandSign.None;
    
    private enum GameState
    {
        Idle,
        PlayerSelected,
        RevealResult
    }
    
    private GameState currentState = GameState.Idle;
    private Coroutine gameFlowCoroutine;
    
    void Start()
    {
        InitializeGame();
        SetupButtons();
    }
    
    private void InitializeGame()
    {
        playerSign = HandSign.None;
        computerSign = HandSign.None;
        currentState = GameState.Idle;
        
        UpdateUI();
    }
    
    private void SetupButtons()
    {
        if (Button_Rock != null)
            Button_Rock.onClick.AddListener(() => OnPlayerSelect(0));
        if (Button_Paper != null)
            Button_Paper.onClick.AddListener(() => OnPlayerSelect(1));
        if (Button_Scissors != null)
            Button_Scissors.onClick.AddListener(() => OnPlayerSelect(2));
        if (Button_Restart != null)
            Button_Restart.onClick.AddListener(RestartGame);
    }
    
    public void OnPlayerSelect(int signIndex)
    {
        if (currentState != GameState.Idle)
            return;
        
        // 轉換索引為 HandSign
        playerSign = (HandSign)(signIndex + 1); // 0->Rock(1), 1->Paper(2), 2->Scissors(3)
        
        currentState = GameState.PlayerSelected;
        
        // 鎖定按鈕
        SetButtonsInteractable(false);
        
        // 更新 UI
        UpdateUI();
        
        // 開始遊戲流程協程
        if (gameFlowCoroutine != null)
            StopCoroutine(gameFlowCoroutine);
        
        gameFlowCoroutine = StartCoroutine(ProcessGameFlow());
    }
    
    private IEnumerator ProcessGameFlow()
    {
        // 顯示等待訊息
        if (Text_Info != null)
        {
            Text_Info.text = $"玩家已出拳：{GetHandSignName(playerSign)}，等待結果...";
        }
        
        // 3 秒倒數
        float waitTime = 3f;
        float elapsed = 0f;
        
        while (elapsed < waitTime)
        {
            elapsed += Time.deltaTime;
            float remaining = waitTime - elapsed;
            
            if (Text_Info != null)
            {
                Text_Info.text = $"玩家已出拳：{GetHandSignName(playerSign)}，倒數中... {remaining:F1} 秒";
            }
            
            yield return null;
        }
        
        // 電腦隨機選擇
        computerSign = (HandSign)Random.Range(1, 4); // 1=Rock, 2=Paper, 3=Scissors
        
        // 進入結果狀態
        currentState = GameState.RevealResult;
        
        // 判斷勝負
        DetermineWinner();
    }
    
    private void DetermineWinner()
    {
        string resultText = "";
        
        if (playerSign == computerSign)
        {
            resultText = "平手！(Draw)";
        }
        else if (
            (playerSign == HandSign.Rock && computerSign == HandSign.Scissors) ||
            (playerSign == HandSign.Paper && computerSign == HandSign.Rock) ||
            (playerSign == HandSign.Scissors && computerSign == HandSign.Paper)
        )
        {
            resultText = "你贏了！(You Win)";
        }
        else
        {
            resultText = "你輸了！(You Lose)";
        }
        
        // 更新 UI
        if (Text_Info != null)
        {
            Text_Info.text = $"玩家：{GetHandSignName(playerSign)} vs 電腦：{GetHandSignName(computerSign)}\n{resultText}";
        }
        
        // 顯示重玩按鈕
        if (Button_Restart != null)
            Button_Restart.gameObject.SetActive(true);
        
        // 更新選擇顯示
        UpdateChoiceImages();
    }
    
    private void UpdateChoiceImages()
    {
        // 更新玩家選擇顯示
        if (Image_PlayerChoice != null)
        {
            Sprite playerSprite = GetHandSignSprite(playerSign);
            if (playerSprite != null)
                Image_PlayerChoice.sprite = playerSprite;
            Image_PlayerChoice.gameObject.SetActive(true);
        }
        
        // 更新電腦選擇顯示
        if (Image_ComputerChoice != null)
        {
            Sprite computerSprite = GetHandSignSprite(computerSign);
            if (computerSprite != null)
                Image_ComputerChoice.sprite = computerSprite;
            Image_ComputerChoice.gameObject.SetActive(true);
        }
    }
    
    private void UpdateUI()
    {
        switch (currentState)
        {
            case GameState.Idle:
                if (Text_Info != null)
                    Text_Info.text = "請出拳 (模擬辨識)";
                
                SetButtonsInteractable(true);
                if (Button_Restart != null)
                    Button_Restart.gameObject.SetActive(false);
                
                // 隱藏選擇顯示或顯示問號
                if (Image_PlayerChoice != null)
                {
                    if (Sprite_QuestionMark != null)
                        Image_PlayerChoice.sprite = Sprite_QuestionMark;
                    Image_PlayerChoice.gameObject.SetActive(true);
                }
                if (Image_ComputerChoice != null)
                {
                    if (Sprite_QuestionMark != null)
                        Image_ComputerChoice.sprite = Sprite_QuestionMark;
                    Image_ComputerChoice.gameObject.SetActive(true);
                }
                break;
                
            case GameState.PlayerSelected:
                // 按鈕已鎖定，UI 在協程中更新
                break;
                
            case GameState.RevealResult:
                // UI 在 DetermineWinner 中更新
                break;
        }
    }
    
    private void SetButtonsInteractable(bool interactable)
    {
        if (Button_Rock != null)
            Button_Rock.interactable = interactable;
        if (Button_Paper != null)
            Button_Paper.interactable = interactable;
        if (Button_Scissors != null)
            Button_Scissors.interactable = interactable;
    }
    
    private string GetHandSignName(HandSign sign)
    {
        switch (sign)
        {
            case HandSign.Rock:
                return "石頭";
            case HandSign.Paper:
                return "布";
            case HandSign.Scissors:
                return "剪刀";
            default:
                return "未知";
        }
    }
    
    private Sprite GetHandSignSprite(HandSign sign)
    {
        switch (sign)
        {
            case HandSign.Rock:
                return Sprite_Rock;
            case HandSign.Paper:
                return Sprite_Paper;
            case HandSign.Scissors:
                return Sprite_Scissors;
            default:
                return Sprite_QuestionMark;
        }
    }
    
    public void RestartGame()
    {
        // 停止協程
        if (gameFlowCoroutine != null)
        {
            StopCoroutine(gameFlowCoroutine);
            gameFlowCoroutine = null;
        }
        
        // 重置遊戲
        InitializeGame();
    }
}
