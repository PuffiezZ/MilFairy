using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro; // ถ้าใช้ TextMeshPro

public class NetworkErrorHandler : MonoBehaviourPunCallbacks
{
    [Header("UI Reference")]
    [SerializeField] private GameObject[] pages;
    [SerializeField] private GameObject errorPanel;
    [SerializeField] private TextMeshProUGUI errorText;

    private void Awake()
    {
        // ทำให้ Object นี้ไม่ถูกทำลายเมื่อเปลี่ยน Scene (ถ้าต้องการให้ดักฟังทั้งเกม)
        // DontDestroyOnLoad(gameObject);

        if (errorPanel != null) errorPanel.SetActive(false);
    }

    // ฟังก์ชันนี้จะถูกเรียกอัตโนมัติเมื่อเกิดการหลุด หรือเชื่อมต่อไม่สำเร็จ
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning($"NetworkErrorHandler: OnDisconnected เรียกทำงาน สาเหตุ: {cause}");

        string errorMessage = "เกิดข้อผิดพลาดในการเชื่อมต่อ: ";

        switch (cause)
        {
            case DisconnectCause.ExceptionOnConnect:
            case DisconnectCause.DnsExceptionOnConnect:
                errorMessage = "Could not connect to server. Please check your internet connection or firewall settings.";
                break;
            case DisconnectCause.ServerTimeout:
                errorMessage = "Server connection timed out. Please ensure you have a stable internet connection.";
                break;
            case DisconnectCause.MaxCcuReached:
                errorMessage = "Server is currently full. Please try again later.";
                break;
            case DisconnectCause.DisconnectByClientLogic:
                // กรณีที่เราสั่ง Disconnect เอง อาจไม่ต้องโชว์ Error
                errorMessage = "Authentication failed. Invalid AppID or credentials.";
                return;
            default:
                errorMessage += cause.ToString();
                break;
        }

        ShowError(errorMessage);
    }

    private void ShowError(string message)
    {
        foreach (var page in pages)
        {
            page.gameObject.SetActive(false);
        }
        if (errorPanel != null && errorText != null)
        {
            errorText.text = message;
            errorPanel.SetActive(true);
        }
    }

    // ฟังก์ชันสำหรับปุ่ม "ตกลง" ในหน้า Popup
    public void OnClickCloseError()
    {
        errorPanel.SetActive(false);
        // อาจจะสั่งโหลด Scene เมนูหลักใหม่เพื่อให้เริ่มเชื่อมต่อใหม่อีกครั้ง
        // UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}