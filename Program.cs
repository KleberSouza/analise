using System;
using System.IO;
using System.Net.Http;
using System.Diagnostics;
using Newtonsoft.Json;

class Program
{
    static Pessoa[] pessoas; // Vetor para armazenar os dados

    static void Main()
    {
        while (true)
        {
            Console.WriteLine("\n=== MENU ===");
            Console.WriteLine("1 - Gerar arquivo de dados");
            Console.WriteLine("2 - Carregar arquivo de dados");
            Console.WriteLine("3 - Executar pesquisa sequencial");
            Console.WriteLine("0 - Sair");
            Console.Write("Escolha uma opção: ");

            string opcao = Console.ReadLine();
            switch (opcao)
            {
                case "1":
                    GerarArquivoDadosAsync();
                    break;
                case "2":
                    CarregarArquivoDados();
                    break;
                case "3":
                    ExecutarPesquisa();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Opção inválida.");
                    break;
            }
        }
    }

    // 1️⃣ GERAR ARQUIVO DE DADOS
    static void GerarArquivoDadosAsync()
    {
        Console.Write("Quantas pessoas deseja gerar? ");
        if (!int.TryParse(Console.ReadLine(), out int quantidade) || quantidade <= 0)
        {
            Console.WriteLine("Número inválido.");
            return;
        }

        List<Pessoa> listaPessoas = new List<Pessoa>();

        for(int i = 1;i<=quantidade;i++){
            listaPessoas.Add(new Pessoa
                {
                    Codigo = i,
                    Id = i.ToString(),
                    Nome = $"Nome {i}",
                    Email = $"Email {i}",
                    Telefone = $"Telefone {i}",
                    Celular = $"Celular {i}",
                    Localizacao = $"Localizacao {i}",
                    Idade = i%100 + 25,
                    Foto = $"Foto {i}",
                });
        }        

        // Salvar no arquivo
        string caminhoArquivo = "dados.txt";
        File.WriteAllText(caminhoArquivo, JsonConvert.SerializeObject(listaPessoas, Formatting.Indented));

        Console.WriteLine($"✅ Arquivo '{caminhoArquivo}' gerado com {quantidade} registros.");
    }


    // 2️⃣ CARREGAR ARQUIVO DE DADOS NA MEMÓRIA
    static void CarregarArquivoDados()
    {
        string caminhoArquivo = "dados.txt";
        if (!File.Exists(caminhoArquivo))
        {
            Console.WriteLine("Nenhum arquivo de dados encontrado.");
            return;
        }

        string conteudo = File.ReadAllText(caminhoArquivo);
        pessoas = JsonConvert.DeserializeObject<Pessoa[]>(conteudo);

        Console.WriteLine($"Dados carregados! {pessoas.Length} registros armazenados na memória.");
    }

    // 3️⃣ EXECUTAR PESQUISA SEQUENCIAL
    static void ExecutarPesquisa()
    {
        if (pessoas == null || pessoas.Length == 0)
        {
            Console.WriteLine("Nenhum dado carregado na memória. Carregue os dados primeiro.");
            return;
        }

        Console.Write("Digite o Código da pessoa a ser pesquisada: ");
        if (!int.TryParse(Console.ReadLine(), out int codigoProcurado))
        {
            Console.WriteLine("Código inválido.");
            return;
        }

        int comparacoes = 0;
        long memoriaInicial = Process.GetCurrentProcess().WorkingSet64; // Memória usada pelo programa

        Stopwatch cronometro = Stopwatch.StartNew();
        Pessoa resultado = null;

        foreach (var pessoa in pessoas)
        {
            comparacoes++;
            if (pessoa.Codigo == codigoProcurado)
            {
                resultado = pessoa;
                break;
            }
        }

        cronometro.Stop();
        long memoriaFinal = Process.GetCurrentProcess().WorkingSet64; // Memória após a pesquisa
        long picoMemoria = Process.GetCurrentProcess().PeakWorkingSet64; // Pico de memória máxima usada

        if (resultado != null)
        {
            Console.WriteLine("\n🔍 Pessoa encontrada:");
            Console.WriteLine(resultado);
        }
        else
        {
            Console.WriteLine("\n❌ Pessoa não encontrada.");
        }

        // Exibir estatísticas da pesquisa
        Console.WriteLine($"\n🔢 Comparações realizadas: {comparacoes}");
        Console.WriteLine($"⏱️ Tempo de pesquisa: {cronometro.Elapsed.TotalMilliseconds} ms");
        Console.WriteLine($"💾 Memória consumida pelo programa: {memoriaFinal} bytes");
        Console.WriteLine($"🚀 Pico máximo de memória usada: {picoMemoria} bytes");
    }
}

// 🔹 Classe Pessoa para armazenar os dados
class Pessoa
{
    public int Codigo { get; set; } // Código sequencial (1 a N)
    public string Id { get; set; }  // ID único da API
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Telefone { get; set; }
    public string Celular { get; set; }
    public string Localizacao { get; set; }
    public int Idade { get; set; }
    public string Foto { get; set; }

    public override string ToString()
    {
        return $"Código: {Codigo} | ID: {Id}\n{Nome}, {Idade} anos - {Localizacao}\n" +
               $"Contato: {Email}, Tel: {Telefone} / {Celular}\nFoto: {Foto}\n";
    }
}
