using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelWindow : MonoBehaviour
{
//sets variables for the text and image in the object, levelmanager object for setting the correct xp and levels etc
    private Text levelText;
    private Image xpBarImage;
    private LevelManager levelManager;

//transforms objects to the correct numbers
    private void Awake() {
     levelText = transform.Find("Background").Find("LevelText").GetComponent<Text>();
     xpBarImage = transform.Find("xpBar").Find("bar").GetComponent<Image>();

 }

//sets xpbar fill amount to the correct level
    private void setXpBarSize(float xp) {
        xpBarImage.fillAmount = xp;


 }
//sets level number
 private void setLevelNumber (int levelNumber) {
     levelText.text = "Level\n" + (levelNumber);
 }

//uses a levelmanager object to get level and xp from, which is then used to set the images for the methods above.
 public void setLevelManager(LevelManager levelManager) {
     this.levelManager = levelManager;
     setLevelNumber(levelManager.getLevel());
     setXpBarSize(levelManager.getXp());
 }
}
