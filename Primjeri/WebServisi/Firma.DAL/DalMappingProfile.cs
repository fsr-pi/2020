using AutoMapper;
using Firma.DAL.Models;
using Firma.DataContract.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Firma.DAL
{
  public class DalMappingProfile : Profile
  {
    public DalMappingProfile()
    {
      CreateMap<Drzava, DrzavaDto>(); //prilikom dohvata iz baze podataka
      CreateMap<DrzavaDto, Drzava>(); //kod datatables
    }
  }
}
