using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using AuraPlus.Web.Data;
using AuraPlus.Web.Models;

namespace AuraPlus.Web.Repositories;

public class UserRepository : IUserRepository
{
    private readonly OracleDbContext _context;

    public UserRepository(OracleDbContext context)
    {
        _context = context;
    }

    public async Task<Users?> GetByIdAsync(int id)
    {
        return await _context.Users
            .Include(u => u.Equipe)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Users?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.Equipe)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<IEnumerable<Users>> GetAllAsync()
    {
        return await _context.Users
            .Include(u => u.Equipe)
            .Where(u => u.Ativo == '1')
            .ToListAsync();
    }

    public async Task<Users> AddAsync(Users user)
    {
        // Usa a procedure prc_inserir_usuario
        var parameters = new[]
        {
            new OracleParameter("p_nome", OracleDbType.NVarchar2, user.Nome, System.Data.ParameterDirection.Input),
            new OracleParameter("p_senha", OracleDbType.NVarchar2, user.Senha, System.Data.ParameterDirection.Input),
            new OracleParameter("p_email", OracleDbType.NVarchar2, user.Email, System.Data.ParameterDirection.Input),
            new OracleParameter("p_role", OracleDbType.NVarchar2, user.Role, System.Data.ParameterDirection.Input),
            new OracleParameter("p_cargo", OracleDbType.NVarchar2, (object?)user.Cargo ?? DBNull.Value, System.Data.ParameterDirection.Input),
            new OracleParameter("p_id_equipe", OracleDbType.Int32, (object?)user.IdEquipe ?? DBNull.Value, System.Data.ParameterDirection.Input),
            new OracleParameter("p_data_admissao", OracleDbType.TimeStamp, (object?)user.DataAdmissao ?? DBNull.Value, System.Data.ParameterDirection.Input)
        };

        try
        {
            await _context.Database.ExecuteSqlRawAsync(
                "BEGIN prc_inserir_usuario(:p_nome, :p_senha, :p_email, :p_role, :p_cargo, :p_id_equipe, :p_data_admissao); END;",
                parameters);

            // Busca o usuário inserido pelo email
            var insertedUser = await GetByEmailAsync(user.Email);
            return insertedUser ?? user;
        }
        catch (Exception ex)
        {
            // Traduz erros da procedure Oracle
            if (ex.Message.Contains("20001"))
                throw new InvalidOperationException("E-mail inválido. Informe um endereço válido.");
            if (ex.Message.Contains("20002"))
                throw new InvalidOperationException("Já existe um usuário com este e-mail.");
            
            throw new InvalidOperationException($"Erro ao inserir usuário: {ex.Message}");
        }
    }

    public async Task UpdateAsync(Users user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return false;

        user.Ativo = '0';
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        var count = await _context.Users
            .Where(u => u.Email.ToLower() == email.ToLower())
            .CountAsync();
        return count > 0;
    }
}
