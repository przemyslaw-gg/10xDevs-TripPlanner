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
            migrationBuilder.Sql(@"INSERT INTO public.users(
	            email, password_hash, display_name, email_verified, created_at, updated_at)
	            VALUES ('demo1@demo.md', '$2a$12$zte9whgwOlOqy2z6LDMXT.CYUExc.YgiKMgv9CuUZLFs.RbPVe9He', 'demo', false, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);");

            migrationBuilder.Sql(@"INSERT INTO public.locations(
	            name, country, timezone, created_at, updated_at)
	            VALUES ('Rzym', 'Włochy', 'A', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);");
            
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
                    'https://upload.wikimedia.org/wikipedia/commons/thumb/d/de/Colosseo_2020.jpg/960px-Colosseo_2020.jpg',
                    (SELECT id FROM users WHERE email = 'demo1@demo.md'),
                    true, now(), now()
                ),
                (
                    (SELECT id FROM locations WHERE name = 'Rzym'),
                    'Forum Romanum',
                    'Ruiny centrum życia politycznego i społecznego starożytnego Rzymu.',
                    41.8925, 12.4853, 4.6, 98000, 90,
                    'https://upload.wikimedia.org/wikipedia/commons/thumb/6/6a/Foro_Romano_Musei_Capitolini_Roma.jpg/1280px-Foro_Romano_Musei_Capitolini_Roma.jpg',
                    (SELECT id FROM users WHERE email = 'demo1@demo.md'),
                    true, now(), now()
                ),
                (
                    (SELECT id FROM locations WHERE name = 'Rzym'),
                    'Panteon',
                    'Doskonale zachowana świątynia z imponującą kopułą.',
                    41.8986, 12.4769, 4.8, 210000, 45,
                    'https://upload.wikimedia.org/wikipedia/commons/thumb/0/06/Rome_Pantheon_front.jpg/1280px-Rome_Pantheon_front.jpg',
                    (SELECT id FROM users WHERE email = 'demo1@demo.md'),
                    true, now(), now()
                ),
                (
                    (SELECT id FROM locations WHERE name = 'Rzym'),
                    'Fontanna di Trevi',
                    'Najsłynniejsza fontanna Rzymu, do której wrzuca się monetę na szczęście.',
                    41.9009, 12.4833, 4.7, 180000, 30,
                    'https://upload.wikimedia.org/wikipedia/commons/thumb/7/7e/Trevi_Fountain%2C_Rome%2C_Italy_2_-_May_2007.jpg/1280px-Trevi_Fountain%2C_Rome%2C_Italy_2_-_May_2007.jpg',
                    (SELECT id FROM users WHERE email = 'demo1@demo.md'),
                    true, now(), now()
                ),
                (
                    (SELECT id FROM locations WHERE name = 'Rzym'),
                    'Plac Hiszpański',
                    'Słynne schody Hiszpańskie i luksusowa dzielnica.',
                    41.9059, 12.4823, 4.6, 90000, 30,
                    'https://upload.wikimedia.org/wikipedia/commons/thumb/0/03/Spanish_Steps%2C_Rome_%2839662732891%29.jpg/1280px-Spanish_Steps%2C_Rome_%2839662732891%29.jpg',
                    (SELECT id FROM users WHERE email = 'demo1@demo.md'),
                    true, now(), now()
                ),
                (
                    (SELECT id FROM locations WHERE name = 'Rzym'),
                    'Watykan',
                    'Najmniejsze państwo świata i centrum Kościoła katolickiego.',
                    41.9029, 12.4534, 4.8, 250000, 180,
                    'https://upload.wikimedia.org/wikipedia/commons/thumb/4/47/Vatican_City_and_St._Peter_Square_evening_twilight_aerial_view.jpg/1280px-Vatican_City_and_St._Peter_Square_evening_twilight_aerial_view.jpg',
                    (SELECT id FROM users WHERE email = 'demo1@demo.md'),
                    true, now(), now()
                ),
                (
                    (SELECT id FROM locations WHERE name = 'Rzym'),
                    'Bazylika św. Piotra',
                    'Największa świątynia chrześcijańska na świecie.',
                    41.9022, 12.4539, 4.9, 300000, 90,
                    'https://upload.wikimedia.org/wikipedia/commons/thumb/a/a5/Saint_Peter_from_Humberto_I_bridge_Rome.jpg/1280px-Saint_Peter_from_Humberto_I_bridge_Rome.jpg',
                    (SELECT id FROM users WHERE email = 'demo1@demo.md'),
                    true, now(), now()
                ),
                (
                    (SELECT id FROM locations WHERE name = 'Rzym'),
                    'Muzea Watykańskie',
                    'Jedna z największych kolekcji sztuki na świecie.',
                    41.9065, 12.4536, 4.7, 190000, 180,
                    'https://upload.wikimedia.org/wikipedia/commons/thumb/0/0e/Mura_vaticane_-_ingresso_ai_Musei_00410.JPG/500px-Mura_vaticane_-_ingresso_ai_Musei_00410.JPG',
                    (SELECT id FROM users WHERE email = 'demo1@demo.md'),
                    true, now(), now()
                ),
                (
                    (SELECT id FROM locations WHERE name = 'Rzym'),
                    'Kaplica Sykstyńska',
                    'Słynna kaplica z freskami Michała Anioła.',
                    41.9066, 12.4534, 4.9, 230000, 30,
                    'https://upload.wikimedia.org/wikipedia/commons/thumb/4/4a/Chapelle_sixtine2.jpg/1280px-Chapelle_sixtine2.jpg',
                    (SELECT id FROM users WHERE email = 'demo1@demo.md'),
                    true, now(), now()
                ),
                (
                    (SELECT id FROM locations WHERE name = 'Rzym'),
                    'Zamek Świętego Anioła',
                    'Mauzoleum cesarza Hadriana, później twierdza papieska.',
                    41.9031, 12.4663, 4.6, 65000, 90,
                    'https://upload.wikimedia.org/wikipedia/commons/thumb/6/67/Engelsburg_und_Engelsbr%C3%BCcke_abends_%28Zuschnitt%29.jpg/960px-Engelsburg_und_Engelsbr%C3%BCcke_abends_%28Zuschnitt%29.jpg',
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
