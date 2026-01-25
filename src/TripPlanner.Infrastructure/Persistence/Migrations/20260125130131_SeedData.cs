using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripPlanner.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            INSERT INTO public.attractions
            (
                location_id,
                name,
                description,
                latitude,
                longitude,
                rating,
                review_count,
                estimated_duration,
                image_url,
                created_by_user_id,
                is_verified,
                created_at,
                updated_at
            )
            VALUES
            (
                (SELECT id FROM locations WHERE name = 'Rzym'),
                'Koloseum',
                'Największy amfiteatr starożytnego Rzymu, symbol potęgi Imperium.',
                41.8902, 12.4922, 4.7, 145000, 120,
                'https://upload.wikimedia.org/wikipedia/commons/d/de/Colosseo_2020.jpg',
                (SELECT id FROM users WHERE email = 'demo1@demo.md'),
                true, now(), now()
            ),
            (
                (SELECT id FROM locations WHERE name = 'Rzym'),
                'Forum Romanum',
                'Ruiny centrum życia politycznego i społecznego starożytnego Rzymu.',
                41.8925, 12.4853, 4.6, 98000, 90,
                'https://upload.wikimedia.org/wikipedia/commons/6/6a/Roman_Forum_Rome.jpg',
                (SELECT id FROM users WHERE email = 'demo1@demo.md'),
                true, now(), now()
            ),
            (
                (SELECT id FROM locations WHERE name = 'Rzym'),
                'Panteon',
                'Doskonale zachowana świątynia z imponującą kopułą.',
                41.8986, 12.4769, 4.8, 210000, 45,
                'https://upload.wikimedia.org/wikipedia/commons/8/8f/Pantheon_Rome_2015.jpg',
                (SELECT id FROM users WHERE email = 'demo1@demo.md'),
                true, now(), now()
            ),
            (
                (SELECT id FROM locations WHERE name = 'Rzym'),
                'Fontanna di Trevi',
                'Najsłynniejsza fontanna Rzymu, do której wrzuca się monetę na szczęście.',
                41.9009, 12.4833, 4.7, 180000, 30,
                'https://upload.wikimedia.org/wikipedia/commons/f/f4/Trevi_Fountain_Rome.jpg',
                (SELECT id FROM users WHERE email = 'demo1@demo.md'),
                true, now(), now()
            ),
            (
                (SELECT id FROM locations WHERE name = 'Rzym'),
                'Plac Hiszpański',
                'Słynne schody Hiszpańskie i luksusowa dzielnica.',
                41.9059, 12.4823, 4.6, 90000, 30,
                'https://upload.wikimedia.org/wikipedia/commons/9/9e/Spanish_Steps_Rome.jpg',
                (SELECT id FROM users WHERE email = 'demo1@demo.md'),
                true, now(), now()
            ),
            (
                (SELECT id FROM locations WHERE name = 'Rzym'),
                'Watykan',
                'Najmniejsze państwo świata i centrum Kościoła katolickiego.',
                41.9029, 12.4534, 4.8, 250000, 180,
                'https://upload.wikimedia.org/wikipedia/commons/0/0a/Vatican_City.jpg',
                (SELECT id FROM users WHERE email = 'demo1@demo.md'),
                true, now(), now()
            ),
            (
                (SELECT id FROM locations WHERE name = 'Rzym'),
                'Bazylika św. Piotra',
                'Największa świątynia chrześcijańska na świecie.',
                41.9022, 12.4539, 4.9, 300000, 90,
                'https://upload.wikimedia.org/wikipedia/commons/1/1d/St_Peters_Basilica_Rome.jpg',
                (SELECT id FROM users WHERE email = 'demo1@demo.md'),
                true, now(), now()
            ),
            (
                (SELECT id FROM locations WHERE name = 'Rzym'),
                'Muzea Watykańskie',
                'Jedna z największych kolekcji sztuki na świecie.',
                41.9065, 12.4536, 4.7, 190000, 180,
                'https://upload.wikimedia.org/wikipedia/commons/5/5f/Vatican_Museum.jpg',
                (SELECT id FROM users WHERE email = 'demo1@demo.md'),
                true, now(), now()
            ),
            (
                (SELECT id FROM locations WHERE name = 'Rzym'),
                'Kaplica Sykstyńska',
                'Słynna kaplica z freskami Michała Anioła.',
                41.9066, 12.4534, 4.9, 230000, 30,
                'https://upload.wikimedia.org/wikipedia/commons/3/3b/Sistine_Chapel_ceiling.jpg',
                (SELECT id FROM users WHERE email = 'demo1@demo.md'),
                true, now(), now()
            ),
            (
                (SELECT id FROM locations WHERE name = 'Rzym'),
                'Zamek Świętego Anioła',
                'Mauzoleum cesarza Hadriana, później twierdza papieska.',
                41.9031, 12.4663, 4.6, 65000, 90,
                'https://upload.wikimedia.org/wikipedia/commons/4/4b/Castel_Sant%27Angelo.jpg',
                (SELECT id FROM users WHERE email = 'demo1@demo.md'),
                true, now(), now()
            );");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
