Algoritmo do Banqueiro - Trabalho Prático 1

## Descrição
Nesse trabalho prático, você deverá escrever um programa multithreaded que implemente o
algoritmo do banqueiro discutido na Seção 7.5.3 do livro
1
. Vários clientes solicitam e liberam
recursos do banco. O banqueiro atenderá uma solicitação somente se ela deixar o sistema em
um estado seguro. Uma solicitação que deixe o sistema em um estado inseguro será negada.
Essa tarefa de programação combina três tópicos diferentes: (1) criar múltiplos threads, (2)
prevenir condições de corrida e (3) evitar deadlocks

O programa utiliza:
- **Threads**: Cada cliente é representado por uma thread independente.
- **Exclusão Mútua**: Uso de `locks` para evitar condições de corrida ao acessar as estruturas de dados compartilhadas.
- **Evitação de Deadlock**: Verificação contínua de estado seguro antes de conceder solicitações de recursos.

## Tecnologias Utilizadas
- Linguagem: C#
- Plataforma: .NET Core / .NET 6.0+

## Pré-requisitos
Para compilar e executar este projeto, você precisa ter o [.NET SDK](https://dotnet.microsoft.com/download) instalado em sua máquina.

## Como Compilar
Navegue até a pasta do projeto no terminal e execute o comando abaixo para restaurar as dependências e compilar o executável:
