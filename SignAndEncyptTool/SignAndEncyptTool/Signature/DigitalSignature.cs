using System.Xml.Serialization;

namespace SignAndEncyptTool.Signature;

public class SigningUserDetails
{
    public string SigningUsername { get; set; } = string.Empty;
    public string SigningUserPCName { get; set; } = string.Empty;
}

public class GeneralDocumentInformation
{
    public int SizeInBytes { get; set; } = 0;
    public string Extension { get; set; } = string.Empty;
    public DateTime LastModificationDateAndTime { get; set; }
}

public class SignatureMethod
{
    [XmlAttribute("Algorithm")]
    public string Algorithm { get; set; } = string.Empty;
}

public class CanonicalizationMethod
{
    [XmlAttribute("Alogorithm")]
    public string Algorithm { get; set; } = string.Empty;
}

public class SignedInfo
{
    public CanonicalizationMethod CanonicalizationMethod { get; set; } = new();
}

public class DiggestMethod
{
    [XmlAttribute("Algorithm")]
    public string Algorithm { get; set; } = string.Empty;
}
public class Reference
{
    [XmlElement("DiggestMethod")]
    public DiggestMethod DiggestMethod { get; set; } = new();
    public string DiggestValue { get; set; } = string.Empty;
}

public class SignedSignatureProperties
{
    public DateTime SigningTime { get; set; }
}

//w tej klasie dodac nody
public class SignedProperties
{
    public SignedSignatureProperties SignedSignatureProperties { get; set; } = new();
    public SigningUserDetails SigningUserDetails { get; set; } = new();
    public GeneralDocumentInformation GeneralDocumentInformation { get; set; } = new();
}

public class QualifyingProperties
{
    public SignedProperties SignedProperties { get; set; } = new();
}

public class SignatureObject
{
    public QualifyingProperties QualifyingProperties { get; set; } = new();
}

[XmlRoot(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
public class DigitalSignature
{
    public SignedInfo SignedInfo { get; set; } = new();

    public byte[] SignatureValue { get; set; } = [];

    public SignatureObject Object { get; set; } = new();
}
