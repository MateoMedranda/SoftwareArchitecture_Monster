using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EurekaBank.Server;

public sealed class EurekaDbContext(DbContextOptions<EurekaDbContext> options) : DbContext(options)
{
    public DbSet<Role> Roles => Set<Role>(); public DbSet<Branch> Branches => Set<Branch>(); public DbSet<User> Users => Set<User>();
    public DbSet<Account> Accounts => Set<Account>(); public DbSet<AccountOwner> AccountOwners => Set<AccountOwner>(); public DbSet<BankToken> Tokens => Set<BankToken>(); public DbSet<Movement> Movements => Set<Movement>();
    protected override void OnModelCreating(ModelBuilder m)
    {
        m.Entity<Role>().ToTable("ROL").HasKey(x => x.Id); m.Entity<Role>().Property(x => x.Id).HasColumnName("ROL_ID"); m.Entity<Role>().Property(x => x.Name).HasColumnName("ROL_NOMBRE");
        m.Entity<Branch>().ToTable("SUCURSAL").HasKey(x => x.Id); m.Entity<Branch>().Property(x => x.Id).HasColumnName("SUCURSAL_ID"); m.Entity<Branch>().Property(x => x.Name).HasColumnName("SUCURSAL_NOMBRE");
        m.Entity<User>().ToTable("USUARIO").HasKey(x => x.Id); m.Entity<User>().Property(x => x.Id).HasColumnName("USUARIO_ID"); m.Entity<User>().Property(x => x.RoleId).HasColumnName("ROL_ID"); m.Entity<User>().Property(x => x.FirstName).HasColumnName("USUARIO_NOMBRE"); m.Entity<User>().Property(x => x.LastName).HasColumnName("USUARIO_APELLIDO"); m.Entity<User>().Property(x => x.Phone).HasColumnName("USUARIO_TELEFONO"); m.Entity<User>().Property(x => x.UserName).HasColumnName("USUARIO_USUARIO"); m.Entity<User>().Property(x => x.PasswordHash).HasColumnName("USUARIO_CONTRASEANIA"); m.Entity<User>().HasOne(x => x.Role).WithMany().HasForeignKey(x => x.RoleId);
        m.Entity<Account>().ToTable("CUENTA").HasKey(x => x.Id); m.Entity<Account>().Property(x => x.Id).HasColumnName("CUENTA_ID"); m.Entity<Account>().Property(x => x.Number).HasColumnName("CUENTA_NUMERO"); m.Entity<Account>().Property(x => x.BranchId).HasColumnName("SUCURSAL_ID"); m.Entity<Account>().Property(x => x.Balance).HasColumnName("CUENTA_SALDO").HasPrecision(15,2); m.Entity<Account>().Property(x => x.Status).HasColumnName("CUENTA_ESTADO"); m.Entity<Account>().HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId); m.Entity<Account>().HasIndex(x => x.Number).IsUnique();
        m.Entity<AccountOwner>().ToTable("PERTENECE").HasKey(x => new { x.AccountId, x.UserId }); m.Entity<AccountOwner>().Property(x => x.AccountId).HasColumnName("CUENTA_ID"); m.Entity<AccountOwner>().Property(x => x.UserId).HasColumnName("USUARIO_ID");
        m.Entity<BankToken>().ToTable("TOKEN").HasKey(x => x.Id); m.Entity<BankToken>().Property(x => x.Id).HasColumnName("TOKEN_ID"); m.Entity<BankToken>().Property(x => x.Value).HasColumnName("TOKEN_TOKEN"); m.Entity<BankToken>().Property(x => x.Status).HasColumnName("TOKEN_ESTADO"); m.Entity<BankToken>().Property(x => x.UserId).HasColumnName("USUARIO_ID"); m.Entity<BankToken>().Property(x => x.CreatedAt).HasColumnName("TOKEN_CREADO_EN"); m.Entity<BankToken>().HasIndex(x => x.Value).IsUnique();
        m.Entity<Movement>().ToTable("TRANSACCION").HasKey(x => x.Id); m.Entity<Movement>().Property(x => x.Id).HasColumnName("TRANSACCION_ID"); m.Entity<Movement>().Property(x => x.AccountId).HasColumnName("CUENTA_ID"); m.Entity<Movement>().Property(x => x.Amount).HasColumnName("TRANSACCION_SALDO").HasPrecision(15,2); m.Entity<Movement>().Property(x => x.Type).HasColumnName("TRANSACCION_TIPO"); m.Entity<Movement>().Property(x => x.BalanceBefore).HasColumnName("TRANSACCION_SALDO_ANTERIOR").HasPrecision(15,2); m.Entity<Movement>().Property(x => x.BalanceAfter).HasColumnName("TRANSACCION_SALDO_POSTERIOR").HasPrecision(15,2); m.Entity<Movement>().Property(x => x.CreatedAt).HasColumnName("TRANSACCION_CREADO_EN"); m.Entity<Movement>().Property(x => x.CreatedBy).HasColumnName("TRANSACCION_CREADO_POR");
    }
}
public sealed class Role { [Key] public string Id { get; set; } = ""; public string Name { get; set; } = ""; }
public sealed class Branch { [Key] public string Id { get; set; } = ""; public string Name { get; set; } = ""; }
public sealed class User { [Key] public string Id { get; set; } = ""; public string? RoleId { get; set; } public string FirstName { get; set; } = ""; public string LastName { get; set; } = ""; public string Phone { get; set; } = ""; public string UserName { get; set; } = ""; public string PasswordHash { get; set; } = ""; public Role? Role { get; set; } }
public sealed class Account { [Key] public string Id { get; set; } = ""; public string Number { get; set; } = ""; public string? BranchId { get; set; } public decimal Balance { get; set; } public string Status { get; set; } = ""; public Branch? Branch { get; set; } }
public sealed class AccountOwner { public string AccountId { get; set; } = ""; public string UserId { get; set; } = ""; }
public sealed class BankToken { [Key] public string Id { get; set; } = ""; public string Value { get; set; } = ""; public string Status { get; set; } = ""; public string UserId { get; set; } = ""; public DateTime CreatedAt { get; set; } }
public sealed class Movement { [Key] public string Id { get; set; } = ""; public string AccountId { get; set; } = ""; public decimal Amount { get; set; } public string Type { get; set; } = ""; public decimal BalanceBefore { get; set; } public decimal BalanceAfter { get; set; } public DateTime CreatedAt { get; set; } public string CreatedBy { get; set; } = ""; }
