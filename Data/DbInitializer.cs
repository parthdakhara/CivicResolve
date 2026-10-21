using CivicResolve.Models;

namespace CivicResolve.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Departments.Any())
            {
                return; // Database has already been seeded
            }

            // 1. Seed Departments
            var departments = new List<Department>
            {
                new Department
                {
                    Name = "Solid Waste & Sanitation",
                    Code = "SANIT",
                    Description = "Garbage collection, public bins, street sweeping, and dumping ground hygiene.",
                    HodName = "Dr. Rajesh K. Verma",
                    ContactEmail = "sanitation@municipal.gov.in",
                    ContactPhone = "9823012345",
                    IsActive = true
                },
                new Department
                {
                    Name = "Roads & Civil Infrastructure",
                    Code = "ROADS",
                    Description = "Pothole repairs, footpath maintenance, asphalt laying, and storm drainage covers.",
                    HodName = "Er. Sunita Rao",
                    ContactEmail = "roads@municipal.gov.in",
                    ContactPhone = "9876543210",
                    IsActive = true
                },
                new Department
                {
                    Name = "Water Supply & Sewerage",
                    Code = "WATER",
                    Description = "Potable water pipeline leaks, contaminated supply, low pressure, and sewage overflow.",
                    HodName = "Er. Amit S. Deshmukh",
                    ContactEmail = "watersupply@municipal.gov.in",
                    ContactPhone = "9821098765",
                    IsActive = true
                },
                new Department
                {
                    Name = "Streetlights & Electrical",
                    Code = "ELEC",
                    Description = "Dark streetlights, malfunctioning timers, damaged poles, and exposed live wires.",
                    HodName = "Er. Vikram Nair",
                    ContactEmail = "electrical@municipal.gov.in",
                    ContactPhone = "9765432109",
                    IsActive = true
                },
                new Department
                {
                    Name = "Public Health & Pest Control",
                    Code = "HEALTH",
                    Description = "Mosquito fogging, dengue/malaria outbreak prevention, and stray animal vaccination.",
                    HodName = "Dr. Neha Sharma",
                    ContactEmail = "health@municipal.gov.in",
                    ContactPhone = "9845012398",
                    IsActive = true
                }
            };

            context.Departments.AddRange(departments);
            context.SaveChanges();

            // 2. Seed Categories
            var sanitDept = departments.First(d => d.Code == "SANIT");
            var roadsDept = departments.First(d => d.Code == "ROADS");
            var waterDept = departments.First(d => d.Code == "WATER");
            var elecDept = departments.First(d => d.Code == "ELEC");
            var healthDept = departments.First(d => d.Code == "HEALTH");

            var categories = new List<Category>
            {
                // Sanitation
                new Category { Name = "Uncollected Garbage / Full Public Bin", DepartmentId = sanitDept.Id, SlaHours = 24, Description = "Overflowing waste bin or garbage left on roadside." },
                new Category { Name = "Dead Animal Removal", DepartmentId = sanitDept.Id, SlaHours = 12, Description = "Carcass of animal needing urgent hygienic disposal." },
                new Category { Name = "Illegal Dumping of Construction Waste", DepartmentId = sanitDept.Id, SlaHours = 48, Description = "Debris dumped on open plots or roadways." },

                // Roads
                new Category { Name = "Severe Pothole / Crater", DepartmentId = roadsDept.Id, SlaHours = 48, Description = "Road hazard causing vehicle damage or accidents." },
                new Category { Name = "Broken Footpath / Manhole Cover Missing", DepartmentId = roadsDept.Id, SlaHours = 24, Description = "Pedestrian safety hazard." },
                new Category { Name = "Waterlogging / Blocked Storm Drain", DepartmentId = roadsDept.Id, SlaHours = 24, Description = "Stagnant rainwater causing traffic blockages." },

                // Water
                new Category { Name = "Major Pipeline Burst / Water Leakage", DepartmentId = waterDept.Id, SlaHours = 12, Description = "Fresh drinking water wastage on street or main line." },
                new Category { Name = "Contaminated / Muddy Water Supply", DepartmentId = waterDept.Id, SlaHours = 24, Description = "Tap water smelling foul or containing sediment." },
                new Category { Name = "Sewage Overflow on Street", DepartmentId = waterDept.Id, SlaHours = 18, Description = "Foul-smelling sewer leakage hazardous to public health." },

                // Electrical
                new Category { Name = "Streetlight Not Working (Entire Street Dark)", DepartmentId = elecDept.Id, SlaHours = 36, Description = "Cluster of streetlights off at night." },
                new Category { Name = "Exposed Electric Wire / Tilted Pole", DepartmentId = elecDept.Id, SlaHours = 6, Description = "High-risk shock hazard needing immediate response." },

                // Health
                new Category { Name = "Mosquito Breeding / Fogging Request", DepartmentId = healthDept.Id, SlaHours = 48, Description = "Preventive chemical fogging for stagnant water spots." },
                new Category { Name = "Stray Dog Nuisance / Aggression", DepartmentId = healthDept.Id, SlaHours = 72, Description = "Sterilization and vaccination squad request." }
            };

            context.Categories.AddRange(categories);
            context.SaveChanges();

            // 3. Seed Realistic Grievance Complaints
            var potholeCat = categories.First(c => c.Name.Contains("Pothole"));
            var burstCat = categories.First(c => c.Name.Contains("Pipeline Burst"));
            var garbageCat = categories.First(c => c.Name.Contains("Uncollected Garbage"));
            var lightCat = categories.First(c => c.Name.Contains("Streetlight Not Working"));
            var wireCat = categories.First(c => c.Name.Contains("Exposed Electric Wire"));

            var c1 = new Complaint
            {
                TrackingNumber = "CR-2026-10101",
                CitizenName = "Aarav Patel",
                CitizenEmail = "aarav.patel@example.com",
                CitizenPhone = "9820123456",
                WardNumber = 14,
                Address = "Crossroad 4, Near Gandhi Garden, Ward 14",
                Landmark = "Opposite State Bank ATM",
                DepartmentId = roadsDept.Id,
                CategoryId = potholeCat.Id,
                Priority = ComplaintPriority.High,
                Status = ComplaintStatus.InProgress,
                Title = "Deep dangerous pothole near school crossing",
                Description = "There is a massive pothole roughly 2 feet wide right in front of St. Mary's School entrance. Two two-wheelers have skidded here during rainfall. Please repair urgently before an accident occurs.",
                AssignedOfficer = "Inspector Ramesh G.",
                OfficerRemarks = "Inspected site on Oct 19. Cold-mix asphalt repair team scheduled for road resurfacing.",
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                UpdatedAt = DateTime.UtcNow.AddDays(-1),
                SlaDeadline = DateTime.UtcNow.AddDays(-1).AddHours(48)
            };

            var c2 = new Complaint
            {
                TrackingNumber = "CR-2026-10102",
                CitizenName = "Pooja Kulkarni",
                CitizenEmail = "pooja.k@example.com",
                CitizenPhone = "9890234567",
                WardNumber = 8,
                Address = "Flat 302, Green Avenue, Shivaji Nagar",
                Landmark = "Behind Municipal Market",
                DepartmentId = waterDept.Id,
                CategoryId = burstCat.Id,
                Priority = ComplaintPriority.Critical,
                Status = ComplaintStatus.Resolved,
                Title = "Underground pipeline burst, potable water gushing onto main road",
                Description = "A main 6-inch water supply pipe ruptured early morning. Gallons of clean drinking water are being wasted and water pressure in our colony is zero.",
                AssignedOfficer = "Eng. Nilesh Patil",
                OfficerRemarks = "Emergency valve closed, damaged connector replaced and pressure restored successfully.",
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-4),
                ResolvedAt = DateTime.UtcNow.AddDays(-4),
                SlaDeadline = DateTime.UtcNow.AddDays(-5).AddHours(12)
            };

            var c3 = new Complaint
            {
                TrackingNumber = "CR-2026-10103",
                CitizenName = "Mohammed Farhan",
                CitizenEmail = "farhan.m@example.com",
                CitizenPhone = "9765123890",
                WardNumber = 22,
                Address = "Plot 55, Sector 9, Nehru Road",
                Landmark = "Near Community Hall",
                DepartmentId = sanitDept.Id,
                CategoryId = garbageCat.Id,
                Priority = ComplaintPriority.Medium,
                Status = ComplaintStatus.Submitted,
                Title = "Community dumpster not cleared for 4 days",
                Description = "The municipal garbage truck has not visited our street since Saturday. Waste is spilling over into the pedestrian walkway causing severe stench and stray animals gathering.",
                CreatedAt = DateTime.UtcNow.AddHours(-10),
                SlaDeadline = DateTime.UtcNow.AddHours(14)
            };

            var c4 = new Complaint
            {
                TrackingNumber = "CR-2026-10104",
                CitizenName = "Sneha Iyer",
                CitizenEmail = "sneha.iyer@example.com",
                CitizenPhone = "9845112233",
                WardNumber = 5,
                Address = "Street 12, Shanti Vihar",
                Landmark = "Near Public Library",
                DepartmentId = elecDept.Id,
                CategoryId = lightCat.Id,
                Priority = ComplaintPriority.Low,
                Status = ComplaintStatus.Assigned,
                Title = "Entire 200m stretch of streetlights dark for 3 days",
                Description = "Streetlights from pole no. 14 to 22 are completely non-functional. The lane is pitch dark at night, causing safety concerns for women and elderly residents returning late.",
                AssignedOfficer = "Lineman S. Jadhav",
                OfficerRemarks = "Assigned to regional lighting maintenance depot.",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                SlaDeadline = DateTime.UtcNow.AddDays(1)
            };

            var c5 = new Complaint
            {
                TrackingNumber = "CR-2026-10105",
                CitizenName = "Vikram Singh",
                CitizenEmail = "vikram.singh@example.com",
                CitizenPhone = "9823998877",
                WardNumber = 3,
                Address = "Corner of Market Road, Ward 3",
                Landmark = "Near Bus Stop #4",
                DepartmentId = elecDept.Id,
                CategoryId = wireCat.Id,
                Priority = ComplaintPriority.Critical,
                Status = ComplaintStatus.UnderReview,
                Title = "Exposed hanging cable touching tree branches near bus stop",
                Description = "A snapped low-tension power cable is hanging at chest height from pole #8. It was sparking during morning drizzle. Immediate attention required before anyone touches it.",
                CreatedAt = DateTime.UtcNow.AddHours(-2),
                SlaDeadline = DateTime.UtcNow.AddHours(4)
            };

            context.Complaints.AddRange(new[] { c1, c2, c3, c4, c5 });
            context.SaveChanges();

            // 4. Seed Status Histories
            var histories = new List<ComplaintStatusHistory>
            {
                new ComplaintStatusHistory
                {
                    ComplaintId = c1.Id,
                    OldStatus = ComplaintStatus.Submitted,
                    NewStatus = ComplaintStatus.UnderReview,
                    ActionBy = "Ward Helpdesk",
                    Remarks = "Grievance received and verified with Ward 14 records.",
                    ChangedAt = DateTime.UtcNow.AddDays(-3).AddHours(2)
                },
                new ComplaintStatusHistory
                {
                    ComplaintId = c1.Id,
                    OldStatus = ComplaintStatus.UnderReview,
                    NewStatus = ComplaintStatus.InProgress,
                    ActionBy = "Er. Sunita Rao (HOD)",
                    Remarks = "Assigned to road repair squad. Inspector Ramesh dispatched.",
                    ChangedAt = DateTime.UtcNow.AddDays(-2)
                },
                new ComplaintStatusHistory
                {
                    ComplaintId = c2.Id,
                    OldStatus = ComplaintStatus.Submitted,
                    NewStatus = ComplaintStatus.InProgress,
                    ActionBy = "Water Control Room",
                    Remarks = "Urgent dispatch to site repair team.",
                    ChangedAt = DateTime.UtcNow.AddDays(-5).AddHours(1)
                },
                new ComplaintStatusHistory
                {
                    ComplaintId = c2.Id,
                    OldStatus = ComplaintStatus.InProgress,
                    NewStatus = ComplaintStatus.Resolved,
                    ActionBy = "Eng. Nilesh Patil",
                    Remarks = "Ruptured pipe replaced and clean water supply restored.",
                    ChangedAt = DateTime.UtcNow.AddDays(-4)
                }
            };

            context.ComplaintStatusHistories.AddRange(histories);

            // 5. Seed Citizen Feedback
            var feedback = new CitizenFeedback
            {
                ComplaintId = c2.Id,
                Rating = 5,
                Comments = "Extremely impressed with the rapid response! The pipeline repair was completed within 3 hours of lodging the complaint.",
                SubmittedAt = DateTime.UtcNow.AddDays(-4).AddHours(2)
            };

            context.CitizenFeedbacks.Add(feedback);
            context.SaveChanges();
        }
    }
}
