using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QuanLyQuanAn.Models;

public partial class QuanLyQuanAnContext : DbContext
{
    public QuanLyQuanAnContext()
    {
    }

    public QuanLyQuanAnContext(DbContextOptions<QuanLyQuanAnContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Ban> Bans { get; set; }

    public virtual DbSet<CaLamViec> CaLamViecs { get; set; }

    public virtual DbSet<ChiNhanh> ChiNhanhs { get; set; }

    public virtual DbSet<ChiNhanhNguyenLieuDauVao> ChiNhanhNguyenLieuDauVaos { get; set; }

    public virtual DbSet<ChiTietGoiMon> ChiTietGoiMons { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<HoaDonRutDiem> HoaDonRutDiems { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<KhuyenMai> KhuyenMais { get; set; }

    public virtual DbSet<LoaiMon> LoaiMons { get; set; }

    public virtual DbSet<MonGoi> MonGois { get; set; }

    public virtual DbSet<NguyenLieu> NguyenLieus { get; set; }

    public virtual DbSet<NguyenLieuDauVao> NguyenLieuDauVaos { get; set; }

    public virtual DbSet<NguyenLieuMonGoi> NguyenLieuMonGois { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<PhieuChi> PhieuChis { get; set; }

    public virtual DbSet<VaiTro> VaiTros { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LEDNHUAN\\LEDNHUAN;Initial Catalog=QuanLyQuanAn;Integrated Security=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ban>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Ban__3213E83FF6CA38DC");

            entity.ToTable("Ban");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IdChiNhanh).HasColumnName("id_ChiNhanh");
            entity.Property(e => e.LoaiBan).HasMaxLength(10);
            entity.Property(e => e.TinhTrang).HasMaxLength(20);

            entity.HasOne(d => d.IdChiNhanhNavigation).WithMany(p => p.Bans)
                .HasForeignKey(d => d.IdChiNhanh)
                .HasConstraintName("FK__Ban__id_ChiNhanh__59FA5E80");
        });

        modelBuilder.Entity<CaLamViec>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CaLamVie__3213E83F009D1657");

            entity.ToTable("CaLamViec");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.GhiChu).HasColumnType("text");
            entity.Property(e => e.ThoiGianBatDau).HasColumnType("datetime");
            entity.Property(e => e.ThoiGianKetThuc).HasColumnType("datetime");

            entity.HasMany(d => d.NhanViens).WithMany(p => p.CaLamViecs)
                .UsingEntity<Dictionary<string, object>>(
                    "CaLamViecNhanVien",
                    r => r.HasOne<NhanVien>().WithMany()
                        .HasForeignKey("NhanVienId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CaLamViec__NhanV__5EBF139D"),
                    l => l.HasOne<CaLamViec>().WithMany()
                        .HasForeignKey("CaLamViecId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__CaLamViec__CaLam__5DCAEF64"),
                    j =>
                    {
                        j.HasKey("CaLamViecId", "NhanVienId").HasName("PK__CaLamVie__01E2F94599AC9974");
                        j.ToTable("CaLamViec_NhanVien");
                        j.IndexerProperty<int>("CaLamViecId").HasColumnName("CaLamViec_id");
                        j.IndexerProperty<string>("NhanVienId")
                            .HasMaxLength(10)
                            .HasColumnName("NhanVien_id");
                    });
        });

        modelBuilder.Entity<ChiNhanh>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChiNhanh__3213E83F061D65D4");

            entity.ToTable("ChiNhanh");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AnhDaiDien).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Sdt)
                .HasMaxLength(10)
                .HasColumnName("SDT");
            entity.Property(e => e.TenChiNhanh)
                .IsRequired()
                .HasMaxLength(20);
        });

        modelBuilder.Entity<ChiNhanhNguyenLieuDauVao>(entity =>
        {
            entity.HasKey(e => new { e.ChiNhanhId, e.NguyenLieuDauVaoId }).HasName("PK__ChiNhanh__D7EC8EBACB874010");

            entity.ToTable("ChiNhanh_NguyenLieuDauVao");

            entity.Property(e => e.ChiNhanhId).HasColumnName("ChiNhanh_id");
            entity.Property(e => e.NguyenLieuDauVaoId).HasColumnName("NguyenLieuDauVao_id");
            entity.Property(e => e.NgayNhap).HasColumnType("datetime");

            entity.HasOne(d => d.ChiNhanh).WithMany(p => p.ChiNhanhNguyenLieuDauVaos)
                .HasForeignKey(d => d.ChiNhanhId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiNhanh___ChiNh__5812160E");

            entity.HasOne(d => d.NguyenLieuDauVao).WithMany(p => p.ChiNhanhNguyenLieuDauVaos)
                .HasForeignKey(d => d.NguyenLieuDauVaoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiNhanh___Nguye__59063A47");
        });

        modelBuilder.Entity<ChiTietGoiMon>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChiTietG__3213E83F5E658791");

            entity.ToTable("ChiTietGoiMon");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.GhiChu).HasColumnType("text");
            entity.Property(e => e.IdHoaDon).HasColumnName("id_HoaDon");
            entity.Property(e => e.IdMonGoi).HasColumnName("id_MonGoi");
            entity.Property(e => e.TrangThai).HasMaxLength(10);

            entity.HasOne(d => d.IdHoaDonNavigation).WithMany(p => p.ChiTietGoiMons)
                .HasForeignKey(d => d.IdHoaDon)
                .HasConstraintName("FK__ChiTietGo__id_Ho__656C112C");

            entity.HasOne(d => d.IdMonGoiNavigation).WithMany(p => p.ChiTietGoiMons)
                .HasForeignKey(d => d.IdMonGoi)
                .HasConstraintName("FK__ChiTietGo__id_Mo__66603565");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HoaDon__3213E83F0B171073");

            entity.ToTable("HoaDon");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.GioRa).HasColumnType("datetime");
            entity.Property(e => e.GioVao).HasColumnType("datetime");
            entity.Property(e => e.IdBan).HasColumnName("id_Ban");
            entity.Property(e => e.IdKhachHang)
                .HasMaxLength(10)
                .HasColumnName("id_KhachHang");
            entity.Property(e => e.IdKhuyenMai).HasColumnName("id_KhuyenMai");
            entity.Property(e => e.IdNhanVien)
                .HasMaxLength(10)
                .HasColumnName("id_NhanVien");
            entity.Property(e => e.TrangThai).HasMaxLength(20);

            entity.HasOne(d => d.IdBanNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.IdBan)
                .HasConstraintName("FK__HoaDon__id_Ban__628FA481");

            entity.HasOne(d => d.IdKhachHangNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.IdKhachHang)
                .HasConstraintName("FK__HoaDon__id_Khach__60A75C0F");

            entity.HasOne(d => d.IdKhuyenMaiNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.IdKhuyenMai)
                .HasConstraintName("FK__HoaDon__id_Khuye__619B8048");

            entity.HasOne(d => d.IdNhanVienNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.IdNhanVien)
                .HasConstraintName("FK__HoaDon__id_NhanV__5FB337D6");
        });

        modelBuilder.Entity<HoaDonRutDiem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HoaDonRu__3213E83F78ED43D8");

            entity.ToTable("HoaDonRutDiem");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.GioRa).HasColumnType("datetime");
            entity.Property(e => e.GioVao).HasColumnType("datetime");
            entity.Property(e => e.IdKhachHang)
                .HasMaxLength(10)
                .HasColumnName("id_KhachHang");
            entity.Property(e => e.IdNhanVien)
                .HasMaxLength(10)
                .HasColumnName("id_NhanVien");
            entity.Property(e => e.TrangThai).HasMaxLength(20);

            entity.HasOne(d => d.IdKhachHangNavigation).WithMany(p => p.HoaDonRutDiems)
                .HasForeignKey(d => d.IdKhachHang)
                .HasConstraintName("FK__HoaDonRut__id_Kh__6477ECF3");

            entity.HasOne(d => d.IdNhanVienNavigation).WithMany(p => p.HoaDonRutDiems)
                .HasForeignKey(d => d.IdNhanVien)
                .HasConstraintName("FK__HoaDonRut__id_Nh__6383C8BA");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__KhachHan__3213E83F86AE72BA");

            entity.ToTable("KhachHang");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .HasColumnName("id");
            entity.Property(e => e.AnhDaiDien).HasMaxLength(255);
            entity.Property(e => e.Cccd)
                .HasMaxLength(20)
                .HasColumnName("CCCD");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.GioiTinh).HasMaxLength(5);
            entity.Property(e => e.Gmail).HasMaxLength(40);
            entity.Property(e => e.HangKhachHang).HasMaxLength(30);
            entity.Property(e => e.HoTen).HasMaxLength(30);
            entity.Property(e => e.MatKhau).HasMaxLength(255);
            entity.Property(e => e.NguoiGioiThieu).HasMaxLength(10);
            entity.Property(e => e.Sdt).HasMaxLength(10);
        });

        modelBuilder.Entity<KhuyenMai>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__KhuyenMa__3213E83F20F3625F");

            entity.ToTable("KhuyenMai");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.NgayBatDau).HasColumnType("datetime");
            entity.Property(e => e.NgayKetThuc).HasColumnType("datetime");
            entity.Property(e => e.TenKhuyenMai)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.TrangThai).HasMaxLength(1);
        });

        modelBuilder.Entity<LoaiMon>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LoaiMon__3213E83F672F1664");

            entity.ToTable("LoaiMon");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.TenLoaiMon)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<MonGoi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MonGoi__3213E83FB53D79BC");

            entity.ToTable("MonGoi");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AnhDaiDien).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IdChiNhanh).HasColumnName("id_ChiNhanh");
            entity.Property(e => e.IdLoaiMon).HasColumnName("id_LoaiMon");
            entity.Property(e => e.TenMon)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasOne(d => d.IdChiNhanhNavigation).WithMany(p => p.MonGois)
                .HasForeignKey(d => d.IdChiNhanh)
                .HasConstraintName("FK__MonGoi__id_ChiNh__68487DD7");

            entity.HasOne(d => d.IdLoaiMonNavigation).WithMany(p => p.MonGois)
                .HasForeignKey(d => d.IdLoaiMon)
                .HasConstraintName("FK__MonGoi__id_LoaiM__6754599E");
        });

        modelBuilder.Entity<NguyenLieu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NguyenLi__3213E83FD5AC2903");

            entity.ToTable("NguyenLieu");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AnhDaiDien).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.TenNguyenLieu).HasMaxLength(20);
        });

        modelBuilder.Entity<NguyenLieuDauVao>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NguyenLi__3213E83F86315BE1");

            entity.ToTable("NguyenLieuDauVao");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AnhDaiDien).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.TenNguyenLieuDauVao).HasMaxLength(20);
        });

        modelBuilder.Entity<NguyenLieuMonGoi>(entity =>
        {
            entity.HasKey(e => new { e.NguyenLieuId, e.MonGoiId }).HasName("PK__NguyenLi__15951FED548C8A14");

            entity.ToTable("NguyenLieu_MonGoi");

            entity.Property(e => e.NguyenLieuId).HasColumnName("NguyenLieu_id");
            entity.Property(e => e.MonGoiId).HasColumnName("MonGoi_id");

            entity.HasOne(d => d.MonGoi).WithMany(p => p.NguyenLieuMonGois)
                .HasForeignKey(d => d.MonGoiId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__NguyenLie__MonGo__6A30C649");

            entity.HasOne(d => d.NguyenLieu).WithMany(p => p.NguyenLieuMonGois)
                .HasForeignKey(d => d.NguyenLieuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__NguyenLie__Nguye__693CA210");
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NhanVien__3213E83F8E77F7E3");

            entity.ToTable("NhanVien");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .HasColumnName("id");
            entity.Property(e => e.AnhDaiDien).HasMaxLength(255);
            entity.Property(e => e.Cccd)
                .HasMaxLength(20)
                .HasColumnName("CCCD");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.GioiTinh).HasMaxLength(5);
            entity.Property(e => e.Gmail).HasMaxLength(40);
            entity.Property(e => e.HoTen)
                .IsRequired()
                .HasMaxLength(30);
            entity.Property(e => e.IdChiNhanh).HasColumnName("id_ChiNhanh");
            entity.Property(e => e.IdVaiTro).HasColumnName("id_VaiTro");
            entity.Property(e => e.MatKhau).HasMaxLength(255);
            entity.Property(e => e.Sdt).HasMaxLength(10);

            entity.HasOne(d => d.IdChiNhanhNavigation).WithMany(p => p.NhanViens)
                .HasForeignKey(d => d.IdChiNhanh)
                .HasConstraintName("FK__NhanVien__id_Chi__5BE2A6F2");

            entity.HasOne(d => d.IdVaiTroNavigation).WithMany(p => p.NhanViens)
                .HasForeignKey(d => d.IdVaiTro)
                .HasConstraintName("FK__NhanVien__id_Vai__5AEE82B9");
        });

        modelBuilder.Entity<PhieuChi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PhieuChi__3213E83F568C5F79");

            entity.ToTable("PhieuChi");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.GhiChu).HasColumnType("text");
            entity.Property(e => e.IdNhanVien)
                .HasMaxLength(10)
                .HasColumnName("id_NhanVien");
            entity.Property(e => e.NgayLap).HasColumnType("datetime");

            entity.HasOne(d => d.IdNhanVienNavigation).WithMany(p => p.PhieuChis)
                .HasForeignKey(d => d.IdNhanVien)
                .HasConstraintName("FK__PhieuChi__id_Nha__5CD6CB2B");
        });

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__VaiTro__3213E83F796F92EC");

            entity.ToTable("VaiTro");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.TenVaiTro).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
