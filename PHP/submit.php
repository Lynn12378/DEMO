<?php
header("Access-Control-Allow-Origin: *");
header("Access-Control-Allow-Methods: POST, GET, OPTIONS");
header("Access-Control-Allow-Headers: Content-Type");

include 'Connector.php';

$conn = connectDatabase();

if (!$conn) 
{
    echo "Database connection failed.";
    return;
}

// 檢查是否接收到 POST 請求
if ($_SERVER["REQUEST_METHOD"] == "POST") 
{
    // 獲取來自表單的數據，並確保清理和過濾數據
    $answers = [];
    for ($i = 1; $i <= 15; $i++) 
    {
        if (isset($_POST["q$i"])) 
        {
            $answers[] = intval($_POST["q$i"]); // 將答案轉換為整數並加進數組
        }
    }

    // 將答案數組轉換為JSON格式
    $json_answers = json_encode($answers);

    // 獲取玩家 ID
    $player_id = intval($_POST['player_id']);

    // 獲取反饋意見
    $feedback = $_POST['feedback'];

    // 插入到 MySQL 資料庫中
    $sql = "INSERT INTO bfi_responses (player_id, responses, feedback) VALUES (?, ?, ?)";
    $stmt = $conn->prepare($sql);

    // 綁定參數
    $stmt->bind_param("iss", $player_id, $json_answers, $feedback);

    // 執行插入
    $stmt->execute();

    // 檢查是否成功插入
    if ($stmt->affected_rows > 0) {
        echo "提交成功！";
    } else {
        echo "提交失敗: " . $stmt->error;
    }

    // 關閉準備語句
    $stmt->close();
}

// 關閉連接
$conn->close();
?>

