﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PG.Servisi.resources.podaci.baze
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="GO_RIING_NET")]
	public partial class GO_GRADDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertPREDMETI(PREDMETI instance);
    partial void UpdatePREDMETI(PREDMETI instance);
    partial void DeletePREDMETI(PREDMETI instance);
    #endregion
		
		public GO_GRADDataContext() : 
				base(global::System.Configuration.ConfigurationManager.ConnectionStrings["GO_RIING_NETConnectionString"].ConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public GO_GRADDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public GO_GRADDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public GO_GRADDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public GO_GRADDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<PREDMETI> PREDMETIs
		{
			get
			{
				return this.GetTable<PREDMETI>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.PREDMETI")]
	public partial class PREDMETI : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _IDPredmeta;
		
		private int _IDKorisnika;
		
		private int _IDIzvora;
		
		private System.Nullable<int> _IDKlasifikacije;
		
		private int _IDGrupe;
		
		private int _IDStatusa;
		
		private string _TekstPrijave;
		
		private System.DateTime _DatumPrijave;
		
		private string _NaslovPrijave;
		
		private string _SifraPrijave;
		
		private decimal _Latitude;
		
		private decimal _Longitude;
		
		private string _Ulica;
		
		private string _KucniBroj;
		
		private string _Posta;
		
		private string _Mjesto;
		
		private bool _Javno;
		
		private bool _Anonimno;
		
		private bool _Komentiranje;
		
		private System.Nullable<System.DateTime> _DatumZatvaranja;
		
		private bool _Obrisan;
		
		private string _Tag;
		
		private bool _Novi;
		
		private string _Registracija;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIDPredmetaChanging(int value);
    partial void OnIDPredmetaChanged();
    partial void OnIDKorisnikaChanging(int value);
    partial void OnIDKorisnikaChanged();
    partial void OnIDIzvoraChanging(int value);
    partial void OnIDIzvoraChanged();
    partial void OnIDKlasifikacijeChanging(System.Nullable<int> value);
    partial void OnIDKlasifikacijeChanged();
    partial void OnIDGrupeChanging(int value);
    partial void OnIDGrupeChanged();
    partial void OnIDStatusaChanging(int value);
    partial void OnIDStatusaChanged();
    partial void OnTekstPrijaveChanging(string value);
    partial void OnTekstPrijaveChanged();
    partial void OnDatumPrijaveChanging(System.DateTime value);
    partial void OnDatumPrijaveChanged();
    partial void OnNaslovPrijaveChanging(string value);
    partial void OnNaslovPrijaveChanged();
    partial void OnSifraPrijaveChanging(string value);
    partial void OnSifraPrijaveChanged();
    partial void OnLatitudeChanging(decimal value);
    partial void OnLatitudeChanged();
    partial void OnLongitudeChanging(decimal value);
    partial void OnLongitudeChanged();
    partial void OnUlicaChanging(string value);
    partial void OnUlicaChanged();
    partial void OnKucniBrojChanging(string value);
    partial void OnKucniBrojChanged();
    partial void OnPostaChanging(string value);
    partial void OnPostaChanged();
    partial void OnMjestoChanging(string value);
    partial void OnMjestoChanged();
    partial void OnJavnoChanging(bool value);
    partial void OnJavnoChanged();
    partial void OnAnonimnoChanging(bool value);
    partial void OnAnonimnoChanged();
    partial void OnKomentiranjeChanging(bool value);
    partial void OnKomentiranjeChanged();
    partial void OnDatumZatvaranjaChanging(System.Nullable<System.DateTime> value);
    partial void OnDatumZatvaranjaChanged();
    partial void OnObrisanChanging(bool value);
    partial void OnObrisanChanged();
    partial void OnTagChanging(string value);
    partial void OnTagChanged();
    partial void OnNoviChanging(bool value);
    partial void OnNoviChanged();
    partial void OnRegistracijaChanging(string value);
    partial void OnRegistracijaChanged();
    #endregion
		
		public PREDMETI()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IDPredmeta", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int IDPredmeta
		{
			get
			{
				return this._IDPredmeta;
			}
			set
			{
				if ((this._IDPredmeta != value))
				{
					this.OnIDPredmetaChanging(value);
					this.SendPropertyChanging();
					this._IDPredmeta = value;
					this.SendPropertyChanged("IDPredmeta");
					this.OnIDPredmetaChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IDKorisnika", DbType="Int NOT NULL")]
		public int IDKorisnika
		{
			get
			{
				return this._IDKorisnika;
			}
			set
			{
				if ((this._IDKorisnika != value))
				{
					this.OnIDKorisnikaChanging(value);
					this.SendPropertyChanging();
					this._IDKorisnika = value;
					this.SendPropertyChanged("IDKorisnika");
					this.OnIDKorisnikaChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IDIzvora", DbType="Int NOT NULL")]
		public int IDIzvora
		{
			get
			{
				return this._IDIzvora;
			}
			set
			{
				if ((this._IDIzvora != value))
				{
					this.OnIDIzvoraChanging(value);
					this.SendPropertyChanging();
					this._IDIzvora = value;
					this.SendPropertyChanged("IDIzvora");
					this.OnIDIzvoraChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IDKlasifikacije", DbType="Int")]
		public System.Nullable<int> IDKlasifikacije
		{
			get
			{
				return this._IDKlasifikacije;
			}
			set
			{
				if ((this._IDKlasifikacije != value))
				{
					this.OnIDKlasifikacijeChanging(value);
					this.SendPropertyChanging();
					this._IDKlasifikacije = value;
					this.SendPropertyChanged("IDKlasifikacije");
					this.OnIDKlasifikacijeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IDGrupe", DbType="Int NOT NULL")]
		public int IDGrupe
		{
			get
			{
				return this._IDGrupe;
			}
			set
			{
				if ((this._IDGrupe != value))
				{
					this.OnIDGrupeChanging(value);
					this.SendPropertyChanging();
					this._IDGrupe = value;
					this.SendPropertyChanged("IDGrupe");
					this.OnIDGrupeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IDStatusa", DbType="Int NOT NULL")]
		public int IDStatusa
		{
			get
			{
				return this._IDStatusa;
			}
			set
			{
				if ((this._IDStatusa != value))
				{
					this.OnIDStatusaChanging(value);
					this.SendPropertyChanging();
					this._IDStatusa = value;
					this.SendPropertyChanged("IDStatusa");
					this.OnIDStatusaChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TekstPrijave", DbType="NVarChar(MAX) NOT NULL", CanBeNull=false)]
		public string TekstPrijave
		{
			get
			{
				return this._TekstPrijave;
			}
			set
			{
				if ((this._TekstPrijave != value))
				{
					this.OnTekstPrijaveChanging(value);
					this.SendPropertyChanging();
					this._TekstPrijave = value;
					this.SendPropertyChanged("TekstPrijave");
					this.OnTekstPrijaveChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DatumPrijave", DbType="DateTime NOT NULL")]
		public System.DateTime DatumPrijave
		{
			get
			{
				return this._DatumPrijave;
			}
			set
			{
				if ((this._DatumPrijave != value))
				{
					this.OnDatumPrijaveChanging(value);
					this.SendPropertyChanging();
					this._DatumPrijave = value;
					this.SendPropertyChanged("DatumPrijave");
					this.OnDatumPrijaveChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_NaslovPrijave", DbType="NVarChar(100) NOT NULL", CanBeNull=false)]
		public string NaslovPrijave
		{
			get
			{
				return this._NaslovPrijave;
			}
			set
			{
				if ((this._NaslovPrijave != value))
				{
					this.OnNaslovPrijaveChanging(value);
					this.SendPropertyChanging();
					this._NaslovPrijave = value;
					this.SendPropertyChanged("NaslovPrijave");
					this.OnNaslovPrijaveChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SifraPrijave", DbType="NVarChar(20) NOT NULL", CanBeNull=false)]
		public string SifraPrijave
		{
			get
			{
				return this._SifraPrijave;
			}
			set
			{
				if ((this._SifraPrijave != value))
				{
					this.OnSifraPrijaveChanging(value);
					this.SendPropertyChanging();
					this._SifraPrijave = value;
					this.SendPropertyChanged("SifraPrijave");
					this.OnSifraPrijaveChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Latitude", DbType="Decimal(18,6) NOT NULL")]
		public decimal Latitude
		{
			get
			{
				return this._Latitude;
			}
			set
			{
				if ((this._Latitude != value))
				{
					this.OnLatitudeChanging(value);
					this.SendPropertyChanging();
					this._Latitude = value;
					this.SendPropertyChanged("Latitude");
					this.OnLatitudeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Longitude", DbType="Decimal(18,6) NOT NULL")]
		public decimal Longitude
		{
			get
			{
				return this._Longitude;
			}
			set
			{
				if ((this._Longitude != value))
				{
					this.OnLongitudeChanging(value);
					this.SendPropertyChanging();
					this._Longitude = value;
					this.SendPropertyChanged("Longitude");
					this.OnLongitudeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Ulica", DbType="NVarChar(100) NOT NULL", CanBeNull=false)]
		public string Ulica
		{
			get
			{
				return this._Ulica;
			}
			set
			{
				if ((this._Ulica != value))
				{
					this.OnUlicaChanging(value);
					this.SendPropertyChanging();
					this._Ulica = value;
					this.SendPropertyChanged("Ulica");
					this.OnUlicaChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_KucniBroj", DbType="NVarChar(10) NOT NULL", CanBeNull=false)]
		public string KucniBroj
		{
			get
			{
				return this._KucniBroj;
			}
			set
			{
				if ((this._KucniBroj != value))
				{
					this.OnKucniBrojChanging(value);
					this.SendPropertyChanging();
					this._KucniBroj = value;
					this.SendPropertyChanged("KucniBroj");
					this.OnKucniBrojChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Posta", DbType="NVarChar(10) NOT NULL", CanBeNull=false)]
		public string Posta
		{
			get
			{
				return this._Posta;
			}
			set
			{
				if ((this._Posta != value))
				{
					this.OnPostaChanging(value);
					this.SendPropertyChanging();
					this._Posta = value;
					this.SendPropertyChanged("Posta");
					this.OnPostaChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Mjesto", DbType="NVarChar(100) NOT NULL", CanBeNull=false)]
		public string Mjesto
		{
			get
			{
				return this._Mjesto;
			}
			set
			{
				if ((this._Mjesto != value))
				{
					this.OnMjestoChanging(value);
					this.SendPropertyChanging();
					this._Mjesto = value;
					this.SendPropertyChanged("Mjesto");
					this.OnMjestoChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Javno", DbType="Bit NOT NULL")]
		public bool Javno
		{
			get
			{
				return this._Javno;
			}
			set
			{
				if ((this._Javno != value))
				{
					this.OnJavnoChanging(value);
					this.SendPropertyChanging();
					this._Javno = value;
					this.SendPropertyChanged("Javno");
					this.OnJavnoChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Anonimno", DbType="Bit NOT NULL")]
		public bool Anonimno
		{
			get
			{
				return this._Anonimno;
			}
			set
			{
				if ((this._Anonimno != value))
				{
					this.OnAnonimnoChanging(value);
					this.SendPropertyChanging();
					this._Anonimno = value;
					this.SendPropertyChanged("Anonimno");
					this.OnAnonimnoChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Komentiranje", DbType="Bit NOT NULL")]
		public bool Komentiranje
		{
			get
			{
				return this._Komentiranje;
			}
			set
			{
				if ((this._Komentiranje != value))
				{
					this.OnKomentiranjeChanging(value);
					this.SendPropertyChanging();
					this._Komentiranje = value;
					this.SendPropertyChanged("Komentiranje");
					this.OnKomentiranjeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DatumZatvaranja", DbType="Date")]
		public System.Nullable<System.DateTime> DatumZatvaranja
		{
			get
			{
				return this._DatumZatvaranja;
			}
			set
			{
				if ((this._DatumZatvaranja != value))
				{
					this.OnDatumZatvaranjaChanging(value);
					this.SendPropertyChanging();
					this._DatumZatvaranja = value;
					this.SendPropertyChanged("DatumZatvaranja");
					this.OnDatumZatvaranjaChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Obrisan", DbType="Bit NOT NULL")]
		public bool Obrisan
		{
			get
			{
				return this._Obrisan;
			}
			set
			{
				if ((this._Obrisan != value))
				{
					this.OnObrisanChanging(value);
					this.SendPropertyChanging();
					this._Obrisan = value;
					this.SendPropertyChanged("Obrisan");
					this.OnObrisanChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Tag", DbType="NVarChar(MAX)")]
		public string Tag
		{
			get
			{
				return this._Tag;
			}
			set
			{
				if ((this._Tag != value))
				{
					this.OnTagChanging(value);
					this.SendPropertyChanging();
					this._Tag = value;
					this.SendPropertyChanged("Tag");
					this.OnTagChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Novi", DbType="Bit NOT NULL")]
		public bool Novi
		{
			get
			{
				return this._Novi;
			}
			set
			{
				if ((this._Novi != value))
				{
					this.OnNoviChanging(value);
					this.SendPropertyChanging();
					this._Novi = value;
					this.SendPropertyChanged("Novi");
					this.OnNoviChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Registracija", DbType="NVarChar(20)")]
		public string Registracija
		{
			get
			{
				return this._Registracija;
			}
			set
			{
				if ((this._Registracija != value))
				{
					this.OnRegistracijaChanging(value);
					this.SendPropertyChanging();
					this._Registracija = value;
					this.SendPropertyChanged("Registracija");
					this.OnRegistracijaChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591
