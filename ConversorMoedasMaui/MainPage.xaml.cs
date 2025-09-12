using System.Globalization;

namespace ConversorMoedasMaui
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent(); // Inicializa os componentes visuais da tela
            InitDefaults();        // Define os valores padrão ao abrir o app
        }

        // Dicionário com taxas de conversão para o real (BRL)
        private readonly Dictionary<string, decimal> _toBRL = new()
        {
            { "BRL", 1.00m }, // 1 BRL = 1 BRL
            { "USD", 5.60m }, // 1 USD = 5,60 BRL
            { "EUR", 6.10m }  // 1 EUR = 6,10 BRL
        };

        // Dicionário que define qual cultura usar para cada moeda
        private readonly Dictionary<string, string> _cultureByCurrency = new()
        {
            {"BRL", "pt-BR"},
            {"USD", "en-US"},
            {"EUR", "de-DE"}
        };

        // Define valores iniciais da interface
        void InitDefaults()
        {
            FromPicker.SelectedIndex = IndexOf(FromPicker, "BRL"); // Seleciona "BRL" no menu de origem
            ToPicker.SelectedIndex = IndexOf(ToPicker, "USD");     // Seleciona "USD" no menu de destino

            InfoLabel.Text = "Valores fictícios."; // Mensagem informativa
            ResultLabel.Text = string.Empty;       // Limpa o campo de resultado
        }

        // Retorna o índice de um item dentro de um Picker
        int IndexOf(Picker picker, string item) => picker.Items.IndexOf(item);

        // Troca as moedas selecionadas entre si
        void OnInverterClicked(object sender, EventArgs e)
        {
            var fromIndex = FromPicker.SelectedIndex;
            FromPicker.SelectedIndex = ToPicker.SelectedIndex;
            ToPicker.SelectedIndex = fromIndex;
            // InfoRateHint(); // Comentado
        }

        // Quando o usuário muda a moeda, limpa o resultado
        void OnPickerChanged(object sender, EventArgs e)
        {
            // InfoRateHint(); // Comentado
            ResultLabel.Text = string.Empty;
        }

        // Quando o valor digitado muda
        void OnAmountChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(AmountEntry.Text))
            {
                InfoLabel.Text = "Digite um valor para converter."; // Mostra aviso se campo estiver vazio
            }
            else
            {
                //InfoRateHint(); // Comentado
            }
        }

        // Mostra uma dica com a taxa de conversão entre as moedas selecionadas
        void InfoRateHint()
        {
            var from = GetFrom();
            var to = GetTo();

            if (from is null || to is null) return;

            if (from == null)
            {
                InfoLabel.Text = "Mesma moeda selecionada";
            }
            else
            {
                var rate = Rate(from, to);
                InfoLabel.Text = $"1 {from} = {rate:0.####} {to}"; // Exibe a taxa no formato: 1 BRL = X USD
            }
        }

        // Evento do botão de conversão
        async void OnConvertClicked(object sender, EventArgs e)
        {
            try
            {
                var from = GetFrom();
                var to = GetTo();

                // Verifica se o campo está vazio
                if (string.IsNullOrWhiteSpace(AmountEntry.Text))
                {
                    await DisplayAlert("Atencao", "informe o valor numerico um ", "OK");
                    return;
                }

                // Tenta converter o texto digitado para número
                if (!decimal.TryParse(AmountEntry.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var amount) || amount < 0)
                {
                    await DisplayAlert("Atencao", "valor invalido", "OK");
                    return;
                }

                // Faz a conversão entre moedas
                var result = Convert(from, to, amount);

                // Pega a cultura da moeda de destino para formatar
                var culture = new CultureInfo(_cultureByCurrency[to]);

                // Formata o resultado no estilo da moeda de destino
                var formated = result.ToString("C", culture);

                // Exibe o resultado no formato: 100 BRL = $XX.XX
                ResultLabel.Text = $"{amount} {from} = {formated}";

                // Atualiza a taxa mostrada abaixo
                InfoRateHint();
            }
            catch (Exception ex)
            {
                await DisplayAlert("erro", "falha aom converter", "OK"); // Mensagem de erro genérica
            }
        }

        // Faz a conversão entre duas moedas
        decimal Convert(string from, string to, decimal amount)
        {
            if (from == to) return amount; // Se for a mesma moeda, retorna o mesmo valor

            var brl = amount * _toBRL[from];     // Converte primeiro para BRL
            var result = brl / _toBRL[to];       // Depois converte do BRL para a moeda final
            return decimal.Round(result, 4);     // Arredonda para 4 casas decimais
        }

        // Calcula a taxa de conversão entre duas moedas
        decimal Rate(string from, string to)
        {
            if (from == to) return 1m; // Mesma moeda, taxa = 1
            var brl = 1m * _toBRL[from];  // Pega o valor de 1 unidade da moeda de origem em BRL
            var rate = brl / _toBRL[to];  // Converte para moeda de destino
            return rate;
        }

        // Retorna a moeda selecionada no menu de origem
        string? GetFrom() => FromPicker.SelectedIndex >= 0 ? FromPicker.Items[FromPicker.SelectedIndex] : null;

        // Retorna a moeda selecionada no menu de destino
        string? GetTo() => ToPicker.SelectedIndex >= 0 ? ToPicker.Items[ToPicker.SelectedIndex] : null;

    }
}
