using System;

namespace PoslovniPartner
{
  abstract class PoslovniPartner
  {
    public string MaticniBroj { get; private set; }
    public string AdresaSjedista { get; set; }
    public string AdresaIsporuke { get; set; }


    public PoslovniPartner(string maticniBroj, string adresaSjedista, string adresaIsporuke)
    {
      this.MaticniBroj = maticniBroj;
      // obavlja se validacija preoptere�enom metodom!
      if (!this.ValidacijaMaticnogBroja())
        throw new Exception("Pogre�ka unosa mati�nog broja!");
      this.AdresaSjedista = adresaSjedista;
      this.AdresaIsporuke = adresaIsporuke;
    }

    //Override metode ToString() koja je naslje�ena iz razreda System.Object
    //Ovu metodu nije potrebno implementirati u razredu koji naslje�uje ovaj! 
    //Ukoliko je implementriamo potrebno je dodati klju�nu rije� override!
    public override string ToString()
    {
      return MaticniBroj +
        "\nAdresa Sjedi�ta: " + AdresaSjedista +
        "\nAdresa Isporuke: " + AdresaIsporuke;
    }

    public abstract bool ValidacijaMaticnogBroja(); //Zbog abstract,
                                                    // ovu metodu potrebno je implementirati u razredu koji naslje�uje ovaj! 
                                                    // Tako�er, abstract zna�i da je ne�emo implementirati ovdje!

  }
}