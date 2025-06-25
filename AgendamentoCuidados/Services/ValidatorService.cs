using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AgendamentoCuidados.Data;
using AgendamentoCuidados.Models;

namespace AgendamentoCuidados.Services
{
    public class ValidatorService
    {
        private readonly ApplicationDbContext _database;
        //Talvez seja útil para validar alguma informação vinda do banco. Caso ocorra uma atualização no projeto.

        public ValidatorService(ApplicationDbContext database)
        {
            _database = database;

        }

        public int CalcularIdadeDias(DateTime dataNascimento)
        {
            return (DateTime.Today - dataNascimento).Days;

        }

        public bool ERecemNascido(DateTime dataNascimento)
        {
            return CalcularIdadeDias(dataNascimento) <= 56;

        }

        public bool ESenior(DateTime dataNascimento)
        {
            return CalcularIdadeDias(dataNascimento) >= 2555;

        }

    }

}