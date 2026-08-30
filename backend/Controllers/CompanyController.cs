using Microsoft.AspNetCore.Mvc;
using SaraRose.Api.DTOs;

namespace SaraRose.Api.Controllers;

[ApiController]
[Route("api/company")]
public class CompanyController : ControllerBase
{
    [HttpGet]
    public ActionResult<CompanyDto> Get()
    {
        return Ok(new CompanyDto(
            Name: "SARA ROSE NIGERIA LIMITED",
            YearEstablished: 2012,
            BusinessType: "Trader",
            Industry: "Heavy equipment / construction & industrial equipment",
            HeadOffice: "Km 12, Sagamu–Benin Express Way, Opposite Navy Merchant, Ogun State, Nigeria",
            OperatingLocation: "Sagamu, Ogun State, Nigeria",
            ContactPerson: "Mr. Akram Haider",
            Telephone: "+234 80 6665 1111",
            Email: "contact@sararose.com",
            WebsiteNote: "To be advised",
            About: "SARA ROSE NIGERIA LIMITED has been trading in heavy equipment and machinery since 2012. The company operates from its head office at Km 12, Sagamu–Benin Expressway in Ogun State, supplying the categories of machinery that construction, infrastructure and industrial customers rely on to carry out their work. Our business is defined by a single specialisation: heavy equipment. We deal in the machine categories our customers actually operate — earthmoving, construction, material handling, road and compaction, and heavy transport and lifting — and we deal with our customers directly, through a named point of contact rather than an anonymous process.",
            HowWeWork: "SARA ROSE NIGERIA LIMITED operates as a trading company in the heavy equipment sector. Our role is commercial: we deal in construction and industrial machinery, and we work with each customer to identify the category of machine that fits the requirement in front of them. Every site places different demands on a machine. We begin with the requirement — the work, the ground, the timeline — and advise on the equipment category suited to it. Enquiries are handled by a named contact. Commercial discussions stay clear and accountable, and decisions move at the pace the customer's project needs.",
            Vision: "To be a heavy equipment trading company that customers across Nigeria's construction and industrial sectors return to with confidence.",
            Mission: "To supply heavy equipment and machinery that meets our customers' operational requirements — dealing directly, advising honestly on the machinery we trade in, and building working relationships that outlast any single transaction.",
            Sectors:
            [
                "Construction — sites that move, shape and build ground",
                "Infrastructure — roads, earthworks and public works",
                "Industrial — plants, yards and material handling",
                "Related sectors — operations that depend on machinery"
            ],
            Reasons:
            [
                new ReasonDto("Trading since 2012", "SARA ROSE NIGERIA LIMITED has operated continuously in the heavy equipment sector for more than a decade. Longevity in this industry is earned through repeated dealing, not announced."),
                new ReasonDto("One industry, full focus", "Heavy equipment and machinery is our only line of business. We are not generalists who also happen to sell machines — this sector is the whole of our attention."),
                new ReasonDto("A broad equipment portfolio", "From excavators and bulldozers to forklifts, rollers, dump trucks and cranes, our portfolio spans five categories — so a customer with several machine requirements can raise them all in one conversation."),
                new ReasonDto("Local presence in Ogun State", "A physical head office on the Sagamu–Benin Expressway means customers, suppliers and partners always know where to find us."),
                new ReasonDto("A named point of contact", "Enquiries go to Mr. Akram Haider directly, by phone or email. One person, accountable for the conversation from first enquiry onward.")
            ],
            Values:
            [
                new ValueDto("Reliability", "We say what we can do, and we do what we say. In an industry where a delayed machine stalls an entire site, dependability is the product."),
                new ValueDto("Integrity", "Straight answers on equipment, terms and timelines — including when the answer is not the one a customer hoped for."),
                new ValueDto("Customer focus", "We start with the requirement, not the inventory. Understanding the work comes before recommending the machine."),
                new ValueDto("Professionalism", "Clear communication, prompt responses and orderly dealing at every stage of the enquiry and transaction."),
                new ValueDto("Quality", "Heavy equipment must earn its keep on site. Quality of machinery and quality of service are treated as one standard."),
                new ValueDto("Long-term partnerships", "We would rather be a customer's equipment contact for the next decade than close a single transaction today.")
            ]
        ));
    }
}
