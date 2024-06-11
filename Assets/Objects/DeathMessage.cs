using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MyPhotonMessage;
using Butterfly_NPC;
using Target;

public enum DeathPattern
{
    gameDesign,
    collision,
    fallVoid
}

public class DeathMessage : PlayerManager.PlayerManager
{
    private string[] messages = new string[10];
    private string[] messagesEn = new string[10];
    private MessageSender _message;
    //private Target.PointCounter _pointCounter;
    private Player_collisionNPC _player_NPC;

    public DeathPattern deathPattern;

    private void InitMessage()
    {
        messages[(int)DeathPattern.gameDesign] = " ÇÕ[ÉQÅ[ÉÄÇÃédól]Ç…éEÇ≥ÇÍÇΩ";
        messages[(int)DeathPattern.collision] = " ÇÕâ^ìÆÉGÉlÉãÉMÅ[ÇëÃå±ÇµÇΩ";
        messages[(int)DeathPattern.fallVoid] = " ÇÕìﬁóéÇÃíÍÇ÷óéÇøÇΩ";

        messagesEn[(int)DeathPattern.gameDesign] = " was killed by [Intentional Game Design]";
        messagesEn[(int)DeathPattern.collision] = " experienced kinetic energy";
        messagesEn[(int)DeathPattern.fallVoid] = " fell out of the world";
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        InitMessage();
        _message = GameObject.Find("Photon Controller").GetComponent<MessageSender>();
        //_pointCounter = this.transform.root.GetComponent<Target.PointCounter>();
        _player_NPC = this.transform.root.Find("Model/Model/NPC_Collider").GetComponent<Player_collisionNPC>();

        GameObject description = this.transform.Find("Description").gameObject;
        description.GetComponent<Text>().text = base.playerName + messages[(int)deathPattern];
        _message.SendAllplayerLogMessage(messagesEn[(int)deathPattern]);

        GameObject score = this.transform.Find("Score").gameObject;
        //string message2 = "Score: " + _pointCounter.point;
        string message2 = "Score: " + _player_NPC.GetScore();
        score.GetComponent<Text>().text = message2;
        _message.SendAllplayerLogMessage(" " + message2);
    }

    protected override void Start() { return; }    // âΩÇ‡ÇµÇ»Ç¢

    public void RestartGame()
    {
        if (base.usePUN2)
        {
            GameObject player = this.transform.root.gameObject;
            Rigidbody playerRig = player.GetComponent<Rigidbody>();
            playerRig.transform.position = base.spawnPosition;
            playerRig.velocity = Vector3.zero;
            //_pointCounter.ResetPoint();

            player.transform.Find("Model").gameObject.transform.rotation = Quaternion.identity;
            SampleCharacter3 character = player.GetComponent<SampleCharacter3>();
            if (character == null) { Debug.LogWarning("Unsupported character"); return; }
            character.enabled = true;
            this.gameObject.SetActive(false);
        }
        else
        {
            base.Respawn();
        }
    }

    public void MoveTitle()
    {
        base.LoadStartScene();
    }
}
