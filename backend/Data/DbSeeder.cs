using Microsoft.EntityFrameworkCore;
using SaraRose.Api.Models;

namespace SaraRose.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken cancellationToken = default)
    {
        await SeedHeaderLinksAsync(db, cancellationToken);
        await EnsureRegistrationHeaderLinkAsync(db, cancellationToken);
        await SeedAdminUserAsync(db, cancellationToken);

        if (await db.Categories.AnyAsync(cancellationToken))
        {
            return;
        }

        var earthmoving = Cat(1, "earthmoving", "01", "Earthmoving equipment", "Earthmoving",
            "Excavators, bulldozers and wheel loaders for sites that move, shape and build ground.");
        var construction = Cat(2, "construction", "02", "Construction equipment", "Construction",
            "Backhoe loaders and motor graders for mixed site work, finishing and road formation.");
        var handling = Cat(3, "material-handling", "03", "Material handling equipment", "Material handling",
            "Forklifts and related machinery for plants, yards and material movement.");
        var road = Cat(4, "road-compaction", "04", "Road & compaction equipment", "Road & compaction",
            "Compactors, rollers and skid-steer rollers for surfaces that must hold.");
        var transport = Cat(5, "transport-lifting", "05", "Heavy transport & lifting equipment", "Transport & lifting",
            "Dump trucks and cranes for haulage and lifting on construction and industrial sites.");

        db.Categories.AddRange(earthmoving, construction, handling, road, transport);

        db.Equipment.AddRange(
            Item(earthmoving, 1, "excavators", "Excavators", "Excavator",
                "Hydraulic excavators for digging, trenching, loading and demolition support.",
                "Excavators sit at the centre of earthmoving work: cut, trench, load and place material with a named operator and a clear duty cycle. SARA ROSE deals in this category as a trader — brands, models, bucket sizes and availability are confirmed at enquiry, against the ground and timeline in front of you.",
                "Foundation pits, drainage, bulk excavation, loading of dump trucks."),
            Item(earthmoving, 2, "bulldozers", "Bulldozers", "Bulldozer",
                "Crawler dozers for stripping, spreading, ripping and pushing material on rough ground.",
                "Bulldozers move ground at scale. They strip topsoil, spread fill, open access and hold a grade when the site is still raw. We advise on this category from the requirement first — the work, the ground, the timeline — not from a stock list.",
                "Site opening, bulk push, spreading fill, pioneering access."),
            Item(earthmoving, 3, "wheel-loaders", "Wheel loaders", "Wheel loader",
                "Wheel loaders for stockpile work, loading and yard handling of aggregates and spoil.",
                "Wheel loaders keep material moving between stockpile, truck and plant. They are a fit for yards, batching operations and sites that need fast load-out rather than deep excavation. Specification and availability are confirmed per enquiry.",
                "Loading dump trucks, stockyard work, plant feed."),

            Item(construction, 4, "backhoe-loaders", "Backhoe loaders", "Backhoe loader",
                "Backhoe loaders for mixed utility work where one machine must dig and load.",
                "A backhoe loader is often the first machine a mixed site asks for: trench at the rear, load at the front, move between tasks without calling a second unit. We deal in this category for construction customers who need flexibility on a compact footprint.",
                "Utilities, small earthworks, loading, site support."),
            Item(construction, 5, "motor-graders", "Motor graders", "Motor grader",
                "Motor graders for forming, finishing and maintaining roads and site grades.",
                "Motor graders finish what earthmoving starts: crown, ditch, camber and a running surface that drainage and compaction can hold. We trade this category for infrastructure and public-works customers whose sites live on the grade.",
                "Road formation, site roads, finishing grades, maintenance."),

            Item(handling, 6, "forklifts", "Forklifts", "Forklift",
                "Forklifts for plants, warehouses and yards that move palletised and unit loads.",
                "Material handling is industrial work: plants, yards and stores that cannot wait on a delayed machine. Forklifts in this category are specified to the load, the aisle and the surface. Brands, capacities and availability are confirmed at enquiry.",
                "Warehouses, plants, container yards, stores."),
            Item(handling, 7, "other-machinery", "Other machinery", "Other machinery",
                "Other heavy equipment and handling machinery beyond the listed groups.",
                "Beyond the five working categories, SARA ROSE NIGERIA LIMITED also deals in other heavy equipment and machinery. If the requirement sits outside the listed machine types, raise it directly with the named contact — commercial discussion stays on the actual job, not a catalogue page.",
                "Specialised plant, yard equipment, related industrial machines."),

            Item(road, 8, "compactors-rollers", "Compactors / rollers", "Compactor / roller",
                "Compactors and rollers for soils, sub-base and asphalt that must densify and hold.",
                "Compaction is what makes a surface last. Rollers and compactors close the earthworks cycle on roads, yards and platforms. We deal in this category for infrastructure and construction customers who need the ground to take traffic, not just look finished.",
                "Road construction, platforms, earthworks, asphalt."),
            Item(road, 9, "skid-steer-rollers", "Skid-steer rollers", "Skid-steer roller",
                "Compact skid-steer rollers for tight sites, shoulders and finishing work.",
                "Skid-steer rollers reach ground that a large roller cannot: shoulders, yards, patch work and compact platforms. They sit in the road and compaction group for customers who need density in a smaller envelope.",
                "Tight sites, shoulders, patching, compact platforms."),

            Item(transport, 10, "dump-trucks", "Dump trucks", "Dump truck",
                "Dump trucks for hauling spoil, fill and aggregates between cut, fill and tip.",
                "Heavy transport keeps earthmoving productive. Dump trucks carry what excavators and loaders fill — spoil out, fill in, aggregates to the grade. We trade this category for construction and infrastructure customers whose sites are defined by haul distance as much as by digging.",
                "Bulk haul, quarry-to-site, cut-and-fill, spoil removal."),
            Item(transport, 11, "cranes", "Cranes", "Crane",
                "Cranes for lifting structural, plant and materials on construction and industrial sites.",
                "Lifting is a category of its own: capacity, radius, ground bearing and the lift plan. Cranes in this group are confirmed at enquiry against the load and the site, not assumed from a brochure. Direct dealing with a named contact keeps that conversation accountable.",
                "Structural erection, plant installation, yard lifts.")
        );

        await db.SaveChangesAsync(cancellationToken);
    }

    private static EquipmentCategory Cat(int sortOrder, string slug, string code, string name, string shortName, string summary) =>
        new()
        {
            Slug = slug,
            Code = code,
            Name = name,
            ShortName = shortName,
            Summary = summary,
            SortOrder = sortOrder
        };

    private static EquipmentItem Item(
        EquipmentCategory category,
        int order,
        string slug,
        string name,
        string machineType,
        string summary,
        string description,
        string typicalUse) =>
        new()
        {
            Category = category,
            Slug = slug,
            Name = name,
            MachineType = machineType,
            Summary = summary,
            Description = description,
            TypicalUse = typicalUse,
            AvailabilityNote = "Brands, models, technical specifications, capacities and availability are confirmed on a per-enquiry basis.",
            SortOrder = order
        };

    private static async Task SeedHeaderLinksAsync(AppDbContext db, CancellationToken cancellationToken)
    {
        if (await db.HeaderLinks.AnyAsync(cancellationToken))
        {
            return;
        }

        var now = DateTime.UtcNow;
        db.HeaderLinks.AddRange(
            Link(1, "Home", "/", false, now),
            Link(2, "About", "/about", false, now),
            Link(3, "Equipment", "/equipment", false, now),
            Link(4, "Why us", "/why-sara-rose", false, now),
            Link(5, "Vision", "/vision-values", false, now),
            Link(6, "Registration", "/register", false, now),
            Link(7, "Login", "/login", false, now),
            Link(8, "Dashboard", "/dashboard", false, now),
            Link(9, "Enquire", "/contact", true, now)
        );
        await db.SaveChangesAsync(cancellationToken);
    }

    private static HeaderLink Link(int order, string label, string path, bool cta, DateTime now) =>
        new()
        {
            Label = label,
            Path = path,
            SortOrder = order,
            Visible = true,
            IsCta = cta,
            CreatedAtUtc = now
        };

    private static async Task EnsureRegistrationHeaderLinkAsync(AppDbContext db, CancellationToken cancellationToken)
    {
        if (await db.HeaderLinks.AnyAsync(x => x.Path == "/register", cancellationToken))
        {
            return;
        }

        var login = await db.HeaderLinks.FirstOrDefaultAsync(x => x.Path == "/login", cancellationToken);
        var order = login?.SortOrder ?? 6;
        var later = await db.HeaderLinks.Where(x => x.SortOrder >= order).ToListAsync(cancellationToken);
        foreach (var link in later)
        {
            link.SortOrder += 1;
        }

        db.HeaderLinks.Add(Link(order, "Registration", "/register", false, DateTime.UtcNow));
        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedAdminUserAsync(AppDbContext db, CancellationToken cancellationToken)
    {
        var admin = await db.Registrations.FirstOrDefaultAsync(
            x => x.Email == UserAccountRules.AdminEmail,
            cancellationToken);

        if (admin is null)
        {
            admin = UserAccountRules.AdminSeed();
            db.Registrations.Add(admin);
            await db.SaveChangesAsync(cancellationToken);
        }

        var hasAdminMaster = await db.UserMasters.AnyAsync(
            x => x.Username == UserAccountRules.AdminUsername || x.Email == UserAccountRules.AdminEmail,
            cancellationToken);

        if (!hasAdminMaster)
        {
            db.UserMasters.Add(UserAccountRules.AdminUserMaster(admin.Id));
            await db.SaveChangesAsync(cancellationToken);
        }

        await BackfillUserMastersAsync(db, cancellationToken);
    }

    private static async Task BackfillUserMastersAsync(AppDbContext db, CancellationToken cancellationToken)
    {
        var linkedEmails = (await db.UserMasters
            .Select(x => x.Email)
            .ToListAsync(cancellationToken))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var usedNames = (await db.UserMasters.Select(x => x.Username).ToListAsync(cancellationToken))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var orphans = (await db.Registrations.ToListAsync(cancellationToken))
            .Where(x => !linkedEmails.Contains(x.Email))
            .ToList();

        foreach (var row in orphans)
        {
            var baseName = UserAccountRules.NormalizeUsername(row.Email.Split('@')[0]);
            if (baseName.Length < 3)
            {
                baseName = $"user{row.Id}";
            }

            var username = baseName;
            var suffix = 1;
            while (usedNames.Contains(username))
            {
                username = $"{baseName}{suffix++}";
            }

            usedNames.Add(username);
            db.UserMasters.Add(new UserMaster
            {
                Username = username,
                Email = row.Email,
                FullName = row.FullName,
                Phone = row.Phone,
                Role = row.Role,
                UserType = row.UserType,
                HashPassword = row.PasswordHash,
                NormalPassword = string.Empty,
                RegistrationId = row.Id,
                Active = true,
                CreatedAtUtc = row.CreatedAtUtc
            });
        }

        if (orphans.Count > 0)
        {
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
