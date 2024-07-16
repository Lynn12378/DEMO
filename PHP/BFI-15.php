<?php
$player_id = intval($_GET['player_id']);
?>

<!DOCTYPE html>
<html>
<head>
    <title>BFI-15 問卷</title>
    <style>
        body 
        {
            background-image: url('ZIP_13.jpg');
            background-size: auto 100%; /* 自動寬度，高度100% */
            background-repeat: no-repeat;
            background-position: left center;
            font-family: 'KaiTi', serif;
            margin: 0;
            padding: 0;
        }
        .container 
        {
            width: 45%;
            margin: 10px 10px 0 auto;
            background: rgba(255, 255, 255, 0.8);
            padding: 20px;
            box-shadow: 0 0 15px rgba(0, 0, 0, 0.2);
            border-radius: 10px;
            transition: all 0.3s ease-in-out;
        }
        .container:hover 
        {
            box-shadow: 0 0 25px rgba(0, 0, 0, 0.3);
        }
        h1 
        {
            text-align: center;
            color: #333;
            margin-top: 0;
        }
        .title-text 
        {
            font-family: 'Times New Roman', serif;
        }
        .title-text span 
        {
            font-family: 'KaiTi', serif;
        }
        form {
            margin-top: 20px;
        }
        .question-container 
        {
            margin-left: 7.5%;
            margin-bottom: 30px;
        }
        .question-label 
        {
            font-size: 18px;
            color: black;
            display: block;
            margin-bottom: 15px;
        }
        .question-number 
        {
            font-family: 'Times New Roman', serif;
            font-weight: bold;
        }
        .question-text 
        {
            font-family: 'KaiTi', serif;
        }
        .options 
        {
            display: flex;
            gap: 50px;
        }
        .options label 
        {
            color: #555;
            display: flex;
            align-items: center;
            cursor: pointer;
        }
        .options input 
        {
            margin-right: 5px;
        }
        .submit-btn 
        {
            display: block;
            width: 100%;
            padding: 10px;
            background-color: #5cb85c;
            color: white;
            border: none;
            border-radius: 5px;
            font-size: 18px;
            font-family: 'KaiTi', serif;
            cursor: pointer;
            margin-top: 20px;
            margin-bottom: 10px;
            transition: background-color 0.3s;
        }
        .submit-btn:hover 
        {
            background-color: #4cae4c;
        }
    </style>
    <script>
        function validateForm() 
        {
            const questions = document.querySelectorAll('.options');
            for (let i = 0; i < questions.length; i++) 
            {
                const radios = questions[i].querySelectorAll('input[type="radio"]');
                let isChecked = false;
                for (let j = 0; j < radios.length; j++) 
                {
                    if (radios[j].checked) 
                    {
                        isChecked = true;
                        break;
                    }
                }
                if (!isChecked) 
                {
                    alert('請回答第 ' + (i + 1) + ' 題');
                    return false;
                }
            }
            return true;
        }
    </script>
</head>
<body>
    <div class="container">
        <h1><span class="title-text">BFI-15 </span><span>問卷</span></h1>
        <form action="submit.php" method="post" onsubmit="return validateForm()">
            <?php
            $questions = [
                "1. 我是個健談的人。",
                "2. 我對別人有信賴感。",
                "3. 我是個有條理的人。",
                "4. 我經常感到焦慮。",
                "5. 我有豐富的想像力。",
                "6. 我喜歡與別人交流。",
                "7. 我樂於助人。",
                "8. 我工作努力。",
                "9. 我經常感到緊張。",
                "10. 我對新觀念持開放態度。",
                "11. 我喜歡參加社交活動。",
                "12. 我是個善良的人。",
                "13. 我是個認真負責的人。",
                "14. 我經常感到情緒低落。",
                "15. 我有很多創意。",
            ];

            foreach ($questions as $index => $question) 
            {
                $parts = explode(".", $question, 2);
                $question_number = trim($parts[0]);
                $question_text = trim($parts[1]);

                echo "<div class='question-container'>";
                echo "<label class='question-label' for='q" . ($index + 1) . "'>";
                echo "<span class='question-number'>$question_number.</span> ";
                echo "<span class='question-text'>$question_text</span>";
                echo "</label>";
                echo "<div class='options'>";
                echo "<label><input type='radio' name='q" . ($index + 1) . "' value='1'> 強烈不同意</label>";
                echo "<label><input type='radio' name='q" . ($index + 1) . "' value='2'> 不同意</label>";
                echo "<label><input type='radio' name='q" . ($index + 1) . "' value='3'> 中立</label>";
                echo "<label><input type='radio' name='q" . ($index + 1) . "' value='4'> 同意</label>";
                echo "<label><input type='radio' name='q" . ($index + 1) . "' value='5'> 強烈同意</label>";
                echo "</div>";
                echo "</div>";
            }
            ?>
            <input type="hidden" name="player_id" value="<?php echo $player_id; ?>">
            <input type="submit" value="提交" class="submit-btn">
        </form>
    </div>
</body>
</html>
