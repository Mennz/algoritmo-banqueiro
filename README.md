import os

# Define the content of the README.md
readme_content = """# Algoritmo do Banqueiro - Trabalho Prático 1

## Descrição
Este projeto consiste em uma implementação multithreaded do **Algoritmo do Banqueiro**, desenvolvida para a disciplina de Sistemas Operacionais. O objetivo é simular um sistema que gerencia a alocação de recursos entre múltiplos clientes, garantindo que o sistema permaneça sempre em um **Estado Seguro** e evitando a ocorrência de deadlocks.

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

```bash
dotnet build