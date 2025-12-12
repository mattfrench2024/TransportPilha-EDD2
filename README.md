# TransportPilha-EDD2
# 🚐✨ TransportPilha — Sistema de Gerenciamento de Frota (Estrutura de Dados: PILHA)

**Aplicação Console em C# (.NET Framework)** desenvolvida para simular o funcionamento operacional de uma empresa de fretamento que utiliza **estrutura de dados do tipo PILHA** para controlar entrada e saída de veículos em garagens, viagens entre aeroportos e transporte de passageiros.

Este projeto foi elaborado com fins acadêmicos, fazendo uso de lógica, modelagem orientada a objetos e estruturas de dados clássicas, com foco em **pilhas (LIFO)**.

---

## 🧭 Visão Geral

A empresa opera vans entre os aeroportos **Congonhas ↔ Guarulhos**, ambos com **garagens com acesso único**, onde os veículos estacionam de ré — ou seja:  
**o último veículo a entrar é sempre o primeiro a sair (LIFO)**.

O sistema permite:

- Controle da jornada diária  
- Liberação de viagens conforme regras reais  
- Cadastro de novas garagens e veículos  
- Fluxo completo de veículos entre origens e destinos  
- Registro, listagem e estatísticas de viagens  
- Controle total de passageiros transportados  

---

## 🛠 Funcionalidades Principais

### ✔ Menu Oficial do Sistema  
O programa contém as seguintes operações:

| Opção | Descrição |
|------|-----------|
| **0** | Finalizar |
| **1** | Cadastrar veículo |
| **2** | Cadastrar garagem |
| **3** | Iniciar jornada |
| **4** | Encerrar jornada |
| **5** | Liberar viagem de uma origem para um destino |
| **6** | Listar veículos em uma garagem |
| **7** | Informar quantidade de viagens realizadas |
| **8** | Listar viagens detalhadas entre origens/destinos |
| **9** | Informar quantidade total de passageiros transportados |

---

## 🧩 Arquitetura de Classes

📦 TransportPilha-EDD2
├── 🚐 Veiculo
├── 🅿️ Garagem (usa Pilha)
├── 🧾 Viagem
├── 🧠 SistemaTransporte (engine do sistema)
└── ▶️ Program.cs (interface menu)


### 📘 Detalhamento

| Classe | Responsabilidade |
|--------|------------------|
| **Veiculo** | Identificação, capacidade, passageiros transportados |
| **Garagem** | Estrutura de armazenamento em **Pilha** (Stack) |
| **Viagem** | Registros de origem, destino e passageiros |
| **SistemaTransporte** | Regras, validações e controle operacional |
| **Program.cs** | Console e interface do usuário |

---

## 📦 Estrutura do Repositório
TransportPilha-EDD2/
│
├── Program.cs
├── Veiculo.cs
├── Garagem.cs
├── Viagem.cs
├── SistemaTransporte.cs
├── TransportPilha.csproj
├── TransportPilha.sln
├── .gitignore
└── README.md


---

## ▶️ Como Executar

### 🔧 Requisitos
- **Windows 10/11**
- **Visual Studio 2019+**
- **.NET Framework** (compatível)

### ▶️ Rodar
1. Abra o **Visual Studio**
2. Clique em **Open a project or solution**
3. Selecione `TransportPilha.sln`
4. Pressione **F5**

---

## 🔍 Destaques Técnicos

- Uso completo da estrutura **Stack<T>**
- Operações reais utilizando lógica LIFO
- Sistema robusto com regras operacionais
- Contabilização de passageiros e viagens
- Jornada diária com inicialização/encerramento
- Consistência total entre garagens e fluxos

---

## 📊 Métricas e Relatórios

O sistema permite consultar:

- Total de viagens entre aeroportos  
- Quantidade de passageiros transportados  
- Histórico detalhado de cada viagem  
- Estado completo de veículos em cada garagem  

---

## 🔒 Regras de Negócio Importantes

- A jornada deve ser iniciada antes de qualquer viagem.  
- Nenhum cadastro é permitido durante a jornada.  
- Garagem esvaziada → depende de retorno para reiniciar viagens.  
- Ao encerrar a jornada:  
  - Estatísticas são exibidas  
  - Dados são "limpos" para o próximo dia  

---

## 🧑‍💻 Autor

**Matheus França (mattfrench2024)**  
GitHub: https://github.com/mattfrench2024

---

## 📄 Licença  
Este projeto utiliza a licença **MIT**, permitindo estudos, modificações e evolução do código.

---

## 🏁 Conclusão

O **TransportPilha-EDD2** é uma solução robusta, didática e totalmente alinhada ao estudo de Estruturas de Dados, oferecendo um ambiente simulado realista para compreensão de pilhas, regras de operação e modelagem orientada a objetos em C#.

Sinta-se livre para melhorar o código, contribuir ou adaptar para novos cenários de transporte.

