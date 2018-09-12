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
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="GRADSKO_OKO")]
	public partial class GODataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertGRADOVI1(GRADOVI1 instance);
    partial void UpdateGRADOVI1(GRADOVI1 instance);
    partial void DeleteGRADOVI1(GRADOVI1 instance);
    #endregion
		
		public GODataContext() : 
				base(global::System.Configuration.ConfigurationManager.ConnectionStrings["GRADSKO_OKOConnectionString"].ConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public GODataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public GODataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public GODataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public GODataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<GRADOVI1> GRADOVI1s
		{
			get
			{
				return this.GetTable<GRADOVI1>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.GRADOVI")]
	public partial class GRADOVI1 : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _IDGrada;
		
		private string _NazivGrada;
		
		private string _Baza;
		
		private string _Instanca;
		
		private string _Korisnik;
		
		private string _Lozinka;
		
		private string _Grb;
		
		private decimal _Lattitude;
		
		private decimal _Longitude;
		
		private int _Zoom;
		
		private string _AktivacijskiKod;
		
		private bool _Aktivan;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIDGradaChanging(int value);
    partial void OnIDGradaChanged();
    partial void OnNazivGradaChanging(string value);
    partial void OnNazivGradaChanged();
    partial void OnBazaChanging(string value);
    partial void OnBazaChanged();
    partial void OnInstancaChanging(string value);
    partial void OnInstancaChanged();
    partial void OnKorisnikChanging(string value);
    partial void OnKorisnikChanged();
    partial void OnLozinkaChanging(string value);
    partial void OnLozinkaChanged();
    partial void OnGrbChanging(string value);
    partial void OnGrbChanged();
    partial void OnLattitudeChanging(decimal value);
    partial void OnLattitudeChanged();
    partial void OnLongitudeChanging(decimal value);
    partial void OnLongitudeChanged();
    partial void OnZoomChanging(int value);
    partial void OnZoomChanged();
    partial void OnAktivacijskiKodChanging(string value);
    partial void OnAktivacijskiKodChanged();
    partial void OnAktivanChanging(bool value);
    partial void OnAktivanChanged();
    #endregion
		
		public GRADOVI1()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IDGrada", DbType="Int NOT NULL", IsPrimaryKey=true)]
		public int IDGrada
		{
			get
			{
				return this._IDGrada;
			}
			set
			{
				if ((this._IDGrada != value))
				{
					this.OnIDGradaChanging(value);
					this.SendPropertyChanging();
					this._IDGrada = value;
					this.SendPropertyChanged("IDGrada");
					this.OnIDGradaChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_NazivGrada", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string NazivGrada
		{
			get
			{
				return this._NazivGrada;
			}
			set
			{
				if ((this._NazivGrada != value))
				{
					this.OnNazivGradaChanging(value);
					this.SendPropertyChanging();
					this._NazivGrada = value;
					this.SendPropertyChanged("NazivGrada");
					this.OnNazivGradaChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Baza", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string Baza
		{
			get
			{
				return this._Baza;
			}
			set
			{
				if ((this._Baza != value))
				{
					this.OnBazaChanging(value);
					this.SendPropertyChanging();
					this._Baza = value;
					this.SendPropertyChanged("Baza");
					this.OnBazaChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Instanca", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string Instanca
		{
			get
			{
				return this._Instanca;
			}
			set
			{
				if ((this._Instanca != value))
				{
					this.OnInstancaChanging(value);
					this.SendPropertyChanging();
					this._Instanca = value;
					this.SendPropertyChanged("Instanca");
					this.OnInstancaChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Korisnik", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string Korisnik
		{
			get
			{
				return this._Korisnik;
			}
			set
			{
				if ((this._Korisnik != value))
				{
					this.OnKorisnikChanging(value);
					this.SendPropertyChanging();
					this._Korisnik = value;
					this.SendPropertyChanged("Korisnik");
					this.OnKorisnikChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Lozinka", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string Lozinka
		{
			get
			{
				return this._Lozinka;
			}
			set
			{
				if ((this._Lozinka != value))
				{
					this.OnLozinkaChanging(value);
					this.SendPropertyChanging();
					this._Lozinka = value;
					this.SendPropertyChanged("Lozinka");
					this.OnLozinkaChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Grb", DbType="NVarChar(MAX) NOT NULL", CanBeNull=false)]
		public string Grb
		{
			get
			{
				return this._Grb;
			}
			set
			{
				if ((this._Grb != value))
				{
					this.OnGrbChanging(value);
					this.SendPropertyChanging();
					this._Grb = value;
					this.SendPropertyChanged("Grb");
					this.OnGrbChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Lattitude", DbType="Decimal(18,6) NOT NULL")]
		public decimal Lattitude
		{
			get
			{
				return this._Lattitude;
			}
			set
			{
				if ((this._Lattitude != value))
				{
					this.OnLattitudeChanging(value);
					this.SendPropertyChanging();
					this._Lattitude = value;
					this.SendPropertyChanged("Lattitude");
					this.OnLattitudeChanged();
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
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Zoom", DbType="Int NOT NULL")]
		public int Zoom
		{
			get
			{
				return this._Zoom;
			}
			set
			{
				if ((this._Zoom != value))
				{
					this.OnZoomChanging(value);
					this.SendPropertyChanging();
					this._Zoom = value;
					this.SendPropertyChanged("Zoom");
					this.OnZoomChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AktivacijskiKod", DbType="NVarChar(10) NOT NULL", CanBeNull=false)]
		public string AktivacijskiKod
		{
			get
			{
				return this._AktivacijskiKod;
			}
			set
			{
				if ((this._AktivacijskiKod != value))
				{
					this.OnAktivacijskiKodChanging(value);
					this.SendPropertyChanging();
					this._AktivacijskiKod = value;
					this.SendPropertyChanged("AktivacijskiKod");
					this.OnAktivacijskiKodChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Aktivan", DbType="Bit NOT NULL")]
		public bool Aktivan
		{
			get
			{
				return this._Aktivan;
			}
			set
			{
				if ((this._Aktivan != value))
				{
					this.OnAktivanChanging(value);
					this.SendPropertyChanging();
					this._Aktivan = value;
					this.SendPropertyChanged("Aktivan");
					this.OnAktivanChanged();
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
