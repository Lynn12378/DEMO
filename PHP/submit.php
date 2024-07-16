<?php
$servername = "localhost";
$username = "root";
$password = "";
$dbname = "demo";

// 創建連接
$conn = new mysqli($servername, $username, $password, $dbname);

// 檢查連接是否成功
if ($conn->connect_error) 
{
    die("連接失敗: " . $conn->connect_error);
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

    // 插入到 MySQL 資料庫中
    $sql = "INSERT INTO bfi_responses (player_id, responses) VALUES (?, ?)";
    $stmt = $conn->prepare($sql);

    // 綁定參數
    $stmt->bind_param("is", $player_id, $json_answers);

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

