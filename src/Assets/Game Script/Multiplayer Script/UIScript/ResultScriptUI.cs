using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ResultScriptUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textTitle;
    [SerializeField] private TextMeshProUGUI textTime;
    [SerializeField] private TextMeshProUGUI textSatisfy;
    [SerializeField] private Image fairyImage;
    [SerializeField] private Sprite winFairy;
    [SerializeField] private Sprite loseFairy;
    

    public void OnInvokeResult(float currentHPpercent, bool isWin, float finalTime)
    {
            // 1. แปลงเวลาวินาทีเป็น รูปแบบ นาที:วินาที
        int minutes = Mathf.FloorToInt(finalTime / 60F);
        int seconds = Mathf.FloorToInt(finalTime - minutes * 60);
        string niceTime = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (textTime != null) textTime.text = niceTime;

        // 2. เช็คเงื่อนไขแพ้/ชนะ และข้อความหัวเรื่อง
        if (isWin)
        {
            if (textTitle != null) textTitle.text = "You Win!";
        }
        else
        {
            if (textTitle != null) textTitle.text = "You Lose!";
        }

        // 3. เช็คระดับความพึงพอใจ (Satisfaction) จาก HP Percent
        // (สมมติว่า currentHPpercent ส่งมาเป็นค่า 0.0 - 1.0 หรือ 0 - 100)
        string satisfyMessage = "";
        fairyImage.sprite = isWin ? winFairy : loseFairy; // เปลี่ยนภาพนางฟ้าตามผลแพ้/ชนะ
        if (!isWin || currentHPpercent <= 0)
        {
            satisfyMessage = "Bad!";
        }
        else if (currentHPpercent >= 1f || currentHPpercent >= 100f) // เช็คทั้งแบบ 1.0 และ 100
        {
            satisfyMessage = "Good Job!";
        }
        else if (currentHPpercent > 0.5f || currentHPpercent > 50f)
        {
            satisfyMessage = "Okay!";
        }
        else
        {
            // กรณีอื่นๆ เช่น เลือดเหลือน้อยกว่า 50% แต่ยังชนะอยู่
            satisfyMessage = "Keep Trying!"; 
        }

        if (textSatisfy != null)
        {
            textSatisfy.text = satisfyMessage;
        }
    }
}
