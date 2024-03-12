using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SimpleJSON;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class GameManager : MonoBehaviour
{
    public Text playerStatusText;
    public Text monsterStatusText;
    public Text playerLogText;
    public Text monsterLogText;
    public Image dragonIcon;
    public Image heroIcon;
    public Button attackButton;
    public GameObject gameOverPanel;
    public GameObject splash;
    public Button closeButton;
    public Text resultText;
    public Player player;
    public Monster monster;
    public Log playerLog;
    public Log monsterLog;
    public ParticleSystem sword;
    public AudioSource slash;
    public Save save;
    public cfg.Tables tables;
    public int language_setting;
    public Language language;
    public Button language_toggle;

    private void Start()
    {
        HideGameOverPanel();
        tables = new cfg.Tables(LoadByteBuf);
        save = new();
        save.LoadByJSON();

        // 初始化
        language = new Language();
        if (!PlayerPrefs.HasKey("language")) {
            language_setting = language.GetLanguage();
            PlayerPrefs.SetInt("language",language_setting);
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[language_setting];
        }
        else {
            language_setting = PlayerPrefs.GetInt("language");
            if(language_setting == 0) {
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
                PlayerPrefs.SetInt("language",0);
                Debug.LogWarning("setting " + language_setting);
            }
            else {
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
                PlayerPrefs.SetInt("language",1);
                Debug.LogWarning("setting " + language_setting);
            }
        }

        player = new Player
        {
            Name = tables.TbPlayer.Get(save.playerLevel).Name,
            Level = tables.TbPlayer.Get(save.playerLevel).Level,
            Experience = tables.TbPlayer.Get(save.playerLevel).Exp,
            Health = tables.TbPlayer.Get(save.playerLevel).Hp,
            Attack = tables.TbPlayer.Get(save.playerLevel).Attack,
            ArmorClass = tables.TbPlayer.Get(save.playerLevel).Ac,
            Thac0 = tables.TbPlayer.Get(save.playerLevel).Thac0,
        };
        
        heroIcon.sprite = Resources.Load<Sprite>(tables.TbPlayer.Get(save.playerLevel).Icon);

        monster = new Monster
        {
            Name = tables.TbMonster.Get(save.level).Name,
            Health = tables.TbMonster.Get(save.level).Hp,
            Attack = tables.TbMonster.Get(save.level).Attack,
            ArmorClass = tables.TbMonster.Get(save.level).Ac,
            Thac0 = tables.TbMonster.Get(save.level).Thac0
        };

        dragonIcon.sprite = Resources.Load<Sprite>(tables.TbMonster.Get(save.level).Icon);

        playerLog = new Log
        {
            Text = ""
        };

        monsterLog = new Log
        {
            Text = ""
        };

        // 设置按钮点击事件
        attackButton?.onClick.AddListener(OnAttackButtonClick);
        language_toggle?.onClick.AddListener(SelectLanguage);
        // 更新界面状态
        UpdateUI();
    }
    public void SelectLanguage(){
         if(language_setting == 0) {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
            language_setting = 1;
            PlayerPrefs.SetInt("language",1);
        }
        else {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
            language_setting = 0;
            PlayerPrefs.SetInt("language",0);
        }
        UpdateUI();
    }

    public string GetWord(string index, int language_setting) {
        return language_setting switch
        {
            0 => tables.TbLanguage.Get(index).En,
            1 => tables.TbLanguage.Get(index).Cn,
            _ => tables.TbLanguage.Get(index).En,
        };
    }

    private void NextLevel()
    {

        player.Name = tables.TbPlayer.Get(save.playerLevel).Name;
        player.Level = tables.TbPlayer.Get(save.playerLevel).Level;
        player.Experience = tables.TbPlayer.Get(save.playerLevel).Exp;
        player.Health = tables.TbPlayer.Get(save.playerLevel).Hp;
        player.Attack = tables.TbPlayer.Get(save.playerLevel).Attack;
        player.ArmorClass = tables.TbPlayer.Get(save.playerLevel).Ac;
        player.Thac0 = tables.TbPlayer.Get(save.playerLevel).Thac0;
      
        heroIcon.sprite = Resources.Load<Sprite>(tables.TbPlayer.Get(save.playerLevel).Icon);

        monster.Name = tables.TbMonster.Get(save.level).Name;
        monster.Health = tables.TbMonster.Get(save.level).Hp;
        monster.Attack = tables.TbMonster.Get(save.level).Attack;
        monster.ArmorClass = tables.TbMonster.Get(save.level).Ac;
        monster.Thac0 = tables.TbMonster.Get(save.level).Thac0;

        dragonIcon.sprite = Resources.Load<Sprite>(tables.TbMonster.Get(save.level).Icon);

        UpdateUI();
    }

    private static JSONNode LoadByteBuf(string file)
    {
        return JSON.Parse(Resources.Load("GenerateDatas/json/" + file).ToString());
    }

    private void OnAttackButtonClick()
    {
        LocalizationSettings.StringDatabase.GetAllTables();
        // 玩家攻击怪物逻辑
        int playerAttackRoll = player.RollAttack();
        if (playerAttackRoll == 20)
        {
            int damage = player.RollDamage() * 2; // 根据武器和角色属性计算伤害
            monster.Health -= damage;
            player.GainExperience(10); 
            player.LevelUp();
            save.playerLevel = player.Level;
            save.SaveByJSON(save);
            playerLog.Text = $"{GetWord("攻击检定为", language_setting)} {playerAttackRoll} : {GetWord("致命一击 伤害翻倍", language_setting)}\n" + $"{GetWord("造成", language_setting)} {damage} {GetWord("点伤害", language_setting)}";
        }
        else if (playerAttackRoll == 1)
        {
            playerLog.Text = $"{GetWord("攻击检定为", language_setting)} {playerAttackRoll} : {GetWord("严重失误 无法命中", language_setting)}";
        }
        else if (playerAttackRoll + player.Thac0 >= monster.ArmorClass)
        {
            int damage = player.RollDamage(); // 根据武器和角色属性计算伤害
            monster.Health -= damage;
            player.GainExperience(10); 
            player.LevelUp();
            save.playerLevel = player.Level;
            save.SaveByJSON(save);
            playerLog.Text = $"{GetWord("攻击检定为", language_setting)} {playerAttackRoll} + {player.Thac0} = {playerAttackRoll + player.Thac0} : {GetWord("命中", language_setting)}\n" + $"{GetWord("造成", language_setting)} {damage} {GetWord("点伤害", language_setting)}";

        }
        
        else
        {
            playerLog.Text = $"{GetWord("攻击检定为", language_setting)} {playerAttackRoll} + {player.Thac0} = {playerAttackRoll + player.Thac0} : {GetWord("未命中", language_setting)}";
        }

        // 怪物攻击
        int monsterAttackRoll = monster.RollAttack();
        
        if (monsterAttackRoll == 20)
        {
            int damage = monster.RollDamage() * 2; // 根据武器和角色属性计算伤害
            player.Health -= damage;
            monsterLog.Text = $"{GetWord("攻击检定为", language_setting)} {monsterAttackRoll} : {GetWord("致命一击 伤害翻倍", language_setting)}\n" + $"{GetWord("造成", language_setting)} {damage} {GetWord("点伤害", language_setting)}";
        }
        else if (monsterAttackRoll == 1)
        {
            monsterLog.Text = $"{GetWord("攻击检定为", language_setting)} {monsterAttackRoll} : {GetWord("严重失误 无法命中", language_setting)}";
        }
        else if (monsterAttackRoll + monster.Thac0 >= player.ArmorClass)
        {
            int damage = monster.RollDamage(); // 根据武器和角色属性计算伤害
            player.Health -= damage;
            monsterLog.Text = $"{GetWord("攻击检定为", language_setting)} {monsterAttackRoll} + {monster.Thac0} = {monsterAttackRoll + monster.Thac0} : {GetWord("命中", language_setting)}\n" + $"{GetWord("造成", language_setting)} {damage} {GetWord("点伤害", language_setting)}";
        }
        else
        {
            monsterLog.Text = $"{GetWord("攻击检定为", language_setting)} {monsterAttackRoll} + {monster.Thac0} = {monsterAttackRoll + monster.Thac0} :  {GetWord("未命中", language_setting)}";
        }

        sword.Play();
        slash.Play();
        // 更新界面状态
        CheckHealth();
        UpdateUI();
    }

    public void UpdateUI()
    {
        language_setting = PlayerPrefs.GetInt("language");
        // 更新玩家状态文本
        playerStatusText.text = $"{GetWord(player.Name, language_setting)}\n" +
                                $"{GetWord("等级", language_setting)}: {player.Level}\n" +
                                $"{GetWord("经验", language_setting)}: {player.Experience}\n" +
                                $"{GetWord("防御等级", language_setting)}: {player.ArmorClass}\n" +
                                $"{GetWord("生命值", language_setting)}: {player.Health}\n" +
                                $"{GetWord("攻击力", language_setting)}: {player.Attack}\n" +
                                $"{GetWord("零级命中率", language_setting)}: {player.Thac0}";

        // 更新怪物状态文本
        monsterStatusText.text = $"{GetWord(monster.Name, language_setting)}\n" +
                                $"{GetWord("防御等级", language_setting)}: {monster.ArmorClass}\n" +
                                $"{GetWord("生命值", language_setting)}: {monster.Health}\n" +
                                $"{GetWord("攻击力", language_setting)}: {monster.Attack}\n" +
                                $"{GetWord("零级命中率", language_setting)}: {monster.Thac0}";

        playerLogText.text = $"{playerLog.Text}";

        monsterLogText.text = $"{monsterLog.Text}";
    }

    void HideSplash()
    {
        // 隐藏游戏结束窗口
        splash.SetActive(false);
    }
    void ShowGameOverPanel(string resultMessage)
    {
        // 显示游戏结束窗口，并设置结果文本
        resultText.text = resultMessage;
        gameOverPanel.SetActive(true);

        closeButton?.onClick.AddListener(OnBackgroundPanelClick);
    }

    void HideGameOverPanel()
    {
        // 隐藏游戏结束窗口
        gameOverPanel.SetActive(false);
    }

    // 调用此方法以处理游戏结束
    public void HandleGameOver(string resultMessage)
    {
        // 显示游戏结束窗口
        ShowGameOverPanel(resultMessage);

        // 在此可以添加其他游戏结束的逻辑，例如禁用玩家控制等

    }
    public void OnBackgroundPanelClick()
    {
        // 隐藏游戏结束窗口和背景面板
        HideGameOverPanel();
        save.playerLevel = player.Level;
        if (monster.Health <= 0)
        {
            save.level ++;
            save.SaveByJSON(save);
        }
        NextLevel();
    }
    void CheckHealth()
    {
        // 判断玩家和怪物的健康值是否小于等于0
        if (monster.Health <= 0)
        {
            playerLog.Text = $"{GetWord(player.Name, language_setting)} {GetWord("胜利", language_setting)}";
            HandleGameOver(playerLog.Text);
        }
        else if (player.Health <= 0)
        {
            playerLog.Text = $"{GetWord(player.Name, language_setting)} {GetWord("失败", language_setting)} {GetWord("游戏结束", language_setting)}";
            HandleGameOver(playerLog.Text);
        }
    }
}