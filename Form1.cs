using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;

namespace FlappyBirdGame
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region Variables
        int pipeSpeed = 5; // Boruların hareket hızı
        int gravity = 10;  // Kuşun aşağı çekilme kuvveti (yer çekimi)
        int score = 0;     // Oyunun puanı
        int jumpForce = -8; // Boşluk tuşuna basıldığında kuşun yukarı hareket kuvveti
        int gravityStrength = 8; // Boşluk tuşu bırakıldığında kuşun aşağı çekilme kuvveti 
        #endregion

        #region Game Events
        // Oyun zamanlayıcısının her tetiklendiğinde gerçekleşen olay
        private void gameTimerEvent(object sender, EventArgs e)
        {
            happyBird.Top += gravity; // Kuş yer çekimine göre hareket eder
            pipeBottom.Left -= pipeSpeed; // Borular sola doğru hareket eder
            pipeTop.Left -= pipeSpeed;
            scoureText.Text = "Score: " + score; // Skor güncellenir

            // Borular ekranın dışına çıktığında konumları sıfırlanır ve puan artar
            if (pipeBottom.Left < -150)
            {
                pipeBottom.Left = 800;
                score++;
                PointSound();
            }

            if (pipeTop.Left < -180)
            {
                pipeTop.Left = 950;
                score++;
                PointSound();
            }

            // Kuşun borulara veya zemine çarpıp çarpmadığını kontrol eder
            if (CheckCollision())
            {
                endGame();
            }

            // Skor 5'ten büyükse boruların hızı artar
            if (score > 5)
                pipeSpeed = 10;

            // Kuş ekranın üst kısmına çarptığında oyun biter
            if (happyBird.Top < 5)
                endGame();
            
        }

        // Herhangi bir tuşa basıldığında tetiklenen olay
        private void gameKeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                gravity = jumpForce; // Boşluk tuşuna basıldığında kuş yukarı hareket eder
                FlapSound();
            }
        }

        // Herhangi bir tuş bırakıldığında tetiklenen olay
        private void gameKeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                gravity = gravityStrength; // Boşluk tuşu bırakıldığında kuş aşağı düşmeye başlar
            }
        }
        #endregion

        #region Game Logic
        // Oyunu bitirme fonksiyonu
        private void endGame()
        {
            gameTimer.Stop(); // Zamanlayıcı durdurulur
            scoureText.Text += " Game Over!"; // Oyun bitti mesajı eklenir
            pictureBox3.Image = Properties.Resources.gameOverImage; // Oyun bitti resmi gösterilir
            PlayGameOver();
            happyBird.Image = null; // Kuşun resmi kaldırılır
        }

        // Oyunu yeniden başlatma fonksiyonu
        private void RestartGame()
        {
            happyBird.Location = new Point(12, 179); // Kuş başlangıç pozisyonuna getirilir
            pipeTop.Left = 800;
            pipeBottom.Left = 1200;
            pictureBox3.Image = null; // Oyun bitti resmi kaldırılır
            happyBird.Image = Properties.Resources.flappyImage; // Kuşun resmi geri yüklenir

            score = 0; // Puan sıfırlanır
            pipeSpeed = 5; // Boruların hızı sıfırlanır
            gravity = gravityStrength; // Yer çekimi sıfırlanır
            scoureText.Text = "Score: " + score;
            gameTimer.Start(); // Zamanlayıcı başlatılır
        }

        // Kuşun herhangi bir şeye çarpıp çarpmadığını kontrol eden fonksiyon
        private bool CheckCollision()
        {
            if (happyBird.Bounds.IntersectsWith(pipeBottom.Bounds) ||
               happyBird.Bounds.IntersectsWith(pipeTop.Bounds))
            {
                FlappyBirdSound();
                return true;
            }
            return happyBird.Bounds.IntersectsWith(ground.Bounds);
        }

        // Oyun bittiğinde çalınacak ses
        public async void PlayGameOver()
        {
            await Task.Run(() =>
            {
                // Göreceli dosya yolunu kullanarak ses dosyasını okuma
                string soundFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\dieSound.mp3");

                using (var audioFile = new AudioFileReader(soundFilePath))
                using (var outputDevice = new WaveOutEvent())
                {
                    outputDevice.Init(audioFile);
                    outputDevice.Play();

                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(100); // Ses çalma işlemi bitene kadar beklenir.
                    }
                }
            });
        }

        // Kuş kanat çırptığında çalınacak ses
        public async void FlapSound()
        {
            await Task.Run(() =>
            {
                // Göreceli dosya yolunu kullanarak ses dosyasını okuma
                string soundFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\flapSound.mp3");

                using (var audioFile = new AudioFileReader(soundFilePath))
                using (var outputDevice = new WaveOutEvent())
                {
                    outputDevice.Init(audioFile);
                    outputDevice.Play();

                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(100); // Ses çalma işlemi bitene kadar beklenir.
                    }
                }
            });
        }

        // Puan alındığında çalınacak ses
        public async void PointSound()
        {
            await Task.Run(() =>
            {
                // Göreceli dosya yolunu kullanarak ses dosyasını okuma
                string soundFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\pointSound.mp3");

                using (var audioFile = new AudioFileReader(soundFilePath))
                using (var outputDevice = new WaveOutEvent())
                {
                    outputDevice.Init(audioFile);
                    outputDevice.Play();

                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(100); // Ses çalma işlemi bitene kadar beklenir.
                    }
                }
            });
        }

        // Oyun sırasında çalınacak arka plan müziği
        public async void FlappyBirdSound()
        {
            await Task.Run(() =>
            {
                // Göreceli dosya yolunu kullanarak ses dosyasını okuma
                string soundFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\flappyBirdSound.mp3");

                using (var audioFile = new AudioFileReader(soundFilePath))
                using (var outputDevice = new WaveOutEvent())
                {
                    outputDevice.Init(audioFile);
                    outputDevice.Play();

                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(100); // Ses çalma işlemi bitene kadar beklenir.
                    }
                }
            });
        }
        #endregion

        #region UI Events
        // İlk resim tıklandığında oyunu yeniden başlatan olay
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            RestartGame();
        }

        // İkinci resim tıklandığında oyunu kapatan olay
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}