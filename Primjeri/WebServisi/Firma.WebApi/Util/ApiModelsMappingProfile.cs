using AutoMapper;
using Firma.DataContract.Commands;
using Firma.DataContract.DTOs;
using Firma.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Firma.WebApi.Util
{
  public class ApiModelsMappingProfile : Profile
  {
    public ApiModelsMappingProfile()
    {
      CreateMap<DrzavaDto, Drzava>(); //DTO koji je vraćen iz nižeg sloja -> objekt koji se vraća kao rezultat servisa
      CreateMap<Drzava, AddDrzava>(); //model sa klijenta -> klasa za naredbu dodavanje države
      CreateMap<Drzava, UpdateDrzava>();
    }
  }
}
