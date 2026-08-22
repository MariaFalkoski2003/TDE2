namespace MathGame
{
    public partial class MainPage : ContentPage
    {
        int iLB1;
        int iLB2;
        int iLB3;
        float fR;

        int iAcertouCount;
        int iErrouCount;
        int perguntaAtual = 1;
        int totalPerguntas = 10;
        int pontos;
        int numeroMaximo = 10;
        int segundosRestantes = 30;

        IDispatcherTimer timer;
        Random rand = new Random();

        public MainPage()
        {
            InitializeComponent();

            timer = Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;

            pkDificuldade.SelectedIndex = 0;

            ReiniciarPartida();
        }

        private void pkDificuldade_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pkDificuldade.SelectedIndex == 0)
                numeroMaximo = 10;
            else if (pkDificuldade.SelectedIndex == 1)
                numeroMaximo = 50;
            else
                numeroMaximo = 100;

            ReiniciarPartida();
        }

        private async void btOK_Clicked(object sender, EventArgs e)
        {
            if (!float.TryParse(txR.Text, out float resposta))
            {
                await DisplayAlertAsync("Erro", "Digite um número válido.", "OK");
                return;
            }

            timer.Stop();
            btOK.IsEnabled = false;

            if (resposta == fR)
            {
                iAcertouCount++;

                int pontosBase;

                if (pkDificuldade.SelectedIndex == 0)
                    pontosBase = 10;
                else if (pkDificuldade.SelectedIndex == 1)
                    pontosBase = 20;
                else
                    pontosBase = 30;

                pontos += pontosBase + segundosRestantes;

                lbAcertou.Text = $"Acertos: {iAcertouCount}";
                lbPontos.Text = $"Pontos: {pontos}";
                imR.Source = "win.png";
            }
            else
            {
                iErrouCount++;
                lbErrou.Text = $"Erros: {iErrouCount}";
                imR.Source = "loose.png";
            }

            await Task.Delay(1500);

            AvancarPergunta();
        }

        private async void Timer_Tick(object? sender, EventArgs e)
        {
            segundosRestantes--;
            lbTempo.Text = $"Tempo: {segundosRestantes}";

            if (segundosRestantes <= 10)
                lbTempo.TextColor = Colors.Red;

            if (segundosRestantes <= 0)
            {
                timer.Stop();
                btOK.IsEnabled = false;

                iErrouCount++;
                lbErrou.Text = $"Erros: {iErrouCount}";
                imR.Source = "loose.png";

                await Task.Delay(1500);

                AvancarPergunta();
            }
        }

        private async void AvancarPergunta()
        {
            perguntaAtual++;

            if (perguntaAtual > totalPerguntas)
            {
                string mensagem;

                if (iAcertouCount >= 8)
                    mensagem = "Ótimo desempenho!";
                else if (iAcertouCount >= 5)
                    mensagem = "Bom trabalho!";
                else
                    mensagem = "Continue praticando!";

                await DisplayAlertAsync(
                    "Fim de jogo",
                    $"Acertos: {iAcertouCount}\n" +
                    $"Erros: {iErrouCount}\n" +
                    $"Pontos: {pontos}\n\n" +
                    mensagem,
                    "Jogar novamente");

                ReiniciarPartida();
                return;
            }

            lbQuestao.Text = $"Questão {perguntaAtual} de {totalPerguntas}";
            pbProgresso.Progress = (double)(perguntaAtual - 1) / totalPerguntas;

            GerarJogo();
        }

        private void ReiniciarPartida()
        {
            timer.Stop();

            perguntaAtual = 1;
            iAcertouCount = 0;
            iErrouCount = 0;
            pontos = 0;

            lbQuestao.Text = "Questão 1 de 10";
            lbAcertou.Text = "Acertos: 0";
            lbErrou.Text = "Erros: 0";
            lbPontos.Text = "Pontos: 0";
            pbProgresso.Progress = 0;

            GerarJogo();
        }

        private void GerarJogo()
        {
            iLB2 = rand.Next(1, 5);
            iLB1 = rand.Next(1, numeroMaximo + 1);

            if (iLB2 == 4)
            {
                do
                {
                    iLB3 = rand.Next(1, iLB1 + 1);
                }
                while (iLB1 % iLB3 != 0);
            }
            else
            {
                iLB3 = rand.Next(1, numeroMaximo + 1);
            }

            lb1.Text = iLB1.ToString();
            lb3.Text = iLB3.ToString();

            switch (iLB2)
            {
                case 1:
                    fR = iLB1 + iLB3;
                    lb2.Text = "+";
                    break;

                case 2:
                    fR = iLB1 - iLB3;
                    lb2.Text = "-";
                    break;

                case 3:
                    fR = iLB1 * iLB3;
                    lb2.Text = "×";
                    break;

                case 4:
                    fR = iLB1 / iLB3;
                    lb2.Text = "÷";
                    break;
            }

            txR.Text = "";
            imR.Source = "question.png";

            segundosRestantes = 30;
            lbTempo.Text = "Tempo: 30";
            lbTempo.TextColor = Colors.Black;

            btOK.IsEnabled = true;

            timer.Stop();
            timer.Start();
        }
    }
}