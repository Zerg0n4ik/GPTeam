import pygame
import random
import sys
import math

# Инициализация Pygame
pygame.init()

# Константы
SCREEN_WIDTH = 800
SCREEN_HEIGHT = 650
GRID_SIZE = 4
CELL_SIZE = 80
GRID_OFFSET_X = (SCREEN_WIDTH - GRID_SIZE * CELL_SIZE) // 2
GRID_OFFSET_Y = 150

# Цвета
WHITE = (255, 255, 255)
BLACK = (0, 0, 0)
GREEN = (100, 160, 60)
BLUE = (70, 120, 200)
RED = (200, 80, 80)
GRAY = (220, 220, 220)
GOLD = (255, 200, 50)
DARK_GREEN = (50, 110, 40)
SAND = (240, 220, 160)
WATER = (100, 180, 220)
METEOR = (180, 100, 50)
LAVA = (220, 100, 50)

# Создание окна
screen = pygame.display.set_mode((SCREEN_WIDTH, SCREEN_HEIGHT))
pygame.display.set_caption("Фабрики на острове")
clock = pygame.time.Clock()

# Шрифты
font = pygame.font.SysFont('Arial', 24)
large_font = pygame.font.SysFont('Arial', 32)
button_font = pygame.font.SysFont('Arial', 20, bold=True)


class Factory:
    def __init__(self, row, col):
        self.row = row
        self.col = col
        self.level = 1
        self.health = 1
        self.max_insurance = 3
        self.x = GRID_OFFSET_X + col * CELL_SIZE + CELL_SIZE // 2
        self.y = GRID_OFFSET_Y + row * CELL_SIZE + CELL_SIZE // 2
        self.animation_offset = 0
        self.animation_direction = 1

    def upgrade(self):
        self.level += 1

    def insure(self):
        if self.health < self.max_insurance:
            self.health += 1

    def take_damage(self):
        self.health -= 1
        return self.health <= 0

    def get_income(self):
        return 5 + (self.level - 1)

    def update_animation(self):
        self.animation_offset += 0.05 * self.animation_direction
        if abs(self.animation_offset) > 2:
            self.animation_direction *= -1

    def draw(self):
        self.update_animation()

        base_size = 30 + self.level * 5
        pygame.draw.rect(screen, BLUE,
                         (self.x - base_size // 2,
                          self.y - base_size // 2 + self.animation_offset,
                          base_size, base_size))

        roof_points = [
            (self.x - base_size // 2, self.y - base_size // 2 + self.animation_offset),
            (self.x, self.y - base_size // 2 - 10 + self.animation_offset),
            (self.x + base_size // 2, self.y - base_size // 2 + self.animation_offset)
        ]
        pygame.draw.polygon(screen, DARK_GREEN, roof_points)

        level_text = font.render(str(self.level), True, WHITE)
        screen.blit(level_text, (self.x - 5, self.y - 10 + self.animation_offset))

        for i in range(self.health):
            heart_x = self.x - 15 + i * 12
            heart_y = self.y + 20 + self.animation_offset
            pygame.draw.polygon(screen, RED, [
                (heart_x, heart_y + 3),
                (heart_x + 3, heart_y),
                (heart_x + 6, heart_y + 3),
                (heart_x + 3, heart_y + 6)
            ])


class Disaster:
    def __init__(self, disaster_type):
        self.type = disaster_type
        self.progress = 0
        self.particles = []
        self.affected_cells = []
        self.prepare_disaster()

    def prepare_disaster(self):
        if self.type == "tornado":
            line_type = random.choice(["row", "col"])
            line_index = random.randint(0, GRID_SIZE - 1)

            for i in range(GRID_SIZE):
                if line_type == "row":
                    self.affected_cells.append((line_index, i))
                else:
                    self.affected_cells.append((i, line_index))

            # Создаем частицы торнадо точно по центру клеток
            for row, col in self.affected_cells:
                x = GRID_OFFSET_X + col * CELL_SIZE + CELL_SIZE // 2
                y = GRID_OFFSET_Y + row * CELL_SIZE + CELL_SIZE // 2
                for _ in range(15):
                    self.particles.append({
                        'x': x,
                        'y': y,
                        'size': random.randint(3, 6),
                        'speed': random.uniform(1, 3),
                        'angle': random.uniform(0, math.pi * 2),
                        'life': random.randint(30, 60)
                    })

        elif self.type == "meteor":
            # Выбираем 4 случайные клетки
            all_cells = [(r, c) for r in range(GRID_SIZE) for c in range(GRID_SIZE)]
            self.affected_cells = random.sample(all_cells, min(4, len(all_cells)))

            # Создаем метеоры, которые падают сверху
            for row, col in self.affected_cells:
                x = GRID_OFFSET_X + col * CELL_SIZE + CELL_SIZE // 2
                y = 0  # Начинаем сверху экрана
                target_y = GRID_OFFSET_Y + row * CELL_SIZE + CELL_SIZE // 2
                self.particles.append({
                    'x': x,
                    'y': y,
                    'target_x': x,
                    'target_y': target_y,
                    'size': 15,
                    'speed': random.uniform(5, 8),
                    'exploded': False
                })

        elif self.type == "flood":
            # Все клетки по краям острова
            for row in range(GRID_SIZE):
                for col in range(GRID_SIZE):
                    if row == 0 or row == GRID_SIZE - 1 or col == 0 or col == GRID_SIZE - 1:
                        self.affected_cells.append((row, col))

            # Создаем волны, которые наступают с краев
            for row, col in self.affected_cells:
                x = GRID_OFFSET_X + col * CELL_SIZE
                y = GRID_OFFSET_Y + row * CELL_SIZE

                # Определяем направление волны
                if row == 0:
                    start_y = y - 50
                    dir_y = 1
                elif row == GRID_SIZE - 1:
                    start_y = y + CELL_SIZE + 50
                    dir_y = -1
                else:
                    start_y = y
                    dir_y = 0

                if col == 0:
                    start_x = x - 50
                    dir_x = 1
                elif col == GRID_SIZE - 1:
                    start_x = x + CELL_SIZE + 50
                    dir_x = -1
                else:
                    start_x = x
                    dir_x = 0

                for _ in range(10):
                    self.particles.append({
                        'x': start_x,
                        'y': start_y,
                        'dir_x': dir_x,
                        'dir_y': dir_y,
                        'speed': random.uniform(1, 3),
                        'size': random.randint(5, 15),
                        'life': random.randint(40, 80)
                    })

    def update(self):
        self.progress += 1

        if self.type == "tornado":
            for p in self.particles:
                p['x'] += math.cos(p['angle']) * p['speed']
                p['y'] += math.sin(p['angle']) * p['speed']
                p['life'] -= 1
                p['size'] *= 0.99

            # Удаляем "мертвые" частицы
            self.particles = [p for p in self.particles if p['life'] > 0]

            return len(self.particles) > 0

        elif self.type == "meteor":
            for p in self.particles:
                if not p['exploded']:
                    # Двигаем метеор к цели
                    dx = p['target_x'] - p['x']
                    dy = p['target_y'] - p['y']
                    dist = max(1, math.sqrt(dx * dx + dy * dy))

                    p['x'] += dx / dist * p['speed']
                    p['y'] += dy / dist * p['speed']

                    # Проверяем достижение цели
                    if dist < 5:
                        p['exploded'] = True
                        p['size'] = 30  # Взрыв
                        p['life'] = 30  # Длительность анимации взрыва
                else:
                    p['life'] -= 1
                    p['size'] *= 0.9

            return any(not p['exploded'] or p['life'] > 0 for p in self.particles)

        elif self.type == "flood":
            for p in self.particles:
                p['x'] += p['dir_x'] * p['speed']
                p['y'] += p['dir_y'] * p['speed']
                p['life'] -= 1

            self.particles = [p for p in self.particles if p['life'] > 0]
            return len(self.particles) > 0

        return False

    def draw(self):
        if self.type == "tornado":
            for p in self.particles:
                alpha = min(255, p['life'] * 4)
                color = (200, 200, 255, alpha)
                s = pygame.Surface((p['size'] * 2, p['size'] * 2), pygame.SRCALPHA)
                pygame.draw.circle(s, color, (p['size'], p['size']), p['size'])
                screen.blit(s, (p['x'] - p['size'], p['y'] - p['size']))



        elif self.type == "meteor":

            for p in self.particles:

                if not p['exploded']:

                    # Рисуем летящий метеор

                    pygame.draw.circle(screen, METEOR, (int(p['x']), int(p['y'])), p['size'])

                    # Хвост метеора

                    tail_length = p['size'] * 2

                    pygame.draw.line(screen, LAVA,

                                     (p['x'], p['y']),

                                     (p['x'] - (p['target_x'] - p['x']) / 10,

                                      p['y'] - (p['target_y'] - p['y']) / 10),

                                     max(2, p['size'] // 2))


        elif self.type == "flood":
            for p in self.particles:
                alpha = min(200, p['life'] * 3)
                s = pygame.Surface((p['size'], p['size']), pygame.SRCALPHA)
                pygame.draw.rect(s, (*WATER, alpha), (0, 0, p['size'], p['size']))
                screen.blit(s, (p['x'], p['y']))


class Game:
    def __init__(self):
        self.coins = 30  # Начальный баланс увеличен
        self.grid = [[None for _ in range(GRID_SIZE)] for _ in range(GRID_SIZE)]
        self.selected_mode = None
        self.build_cost = 15  # Увеличена стоимость постройки
        self.upgrade_cost = 5
        self.insure_cost = 20  # Увеличена стоимость страховки
        self.disaster = None
        self.message = ""
        self.message_timer = 0

        # Кнопки
        self.buttons = [
            {"rect": pygame.Rect(50, 500, 180, 60), "text": "Построить фабрику", "cost": self.build_cost,
             "action": "build", "color": BLUE},
            {"rect": pygame.Rect(250, 500, 180, 60), "text": "Улучшить фабрику", "cost": self.upgrade_cost,
             "action": "upgrade", "color": GREEN},
            {"rect": pygame.Rect(450, 500, 180, 60), "text": "Застраховать", "cost": self.insure_cost,
             "action": "insure", "color": GOLD},
            {"rect": pygame.Rect(650, 500, 120, 60), "text": "Раунд", "cost": 0, "action": "next_round", "color": RED}
        ]

    def show_message(self, text):
        self.message = text
        self.message_timer = 120

    def can_afford(self, cost):
        return self.coins >= cost

    def spend_coins(self, amount):
        if self.can_afford(amount):
            self.coins -= amount
            return True
        self.show_message(f"Недостаточно монет! Нужно {amount}")
        return False

    def add_coins(self, amount):
        self.coins += amount
        self.show_message(f"+{amount} монет")

    def handle_click(self, pos):
        if self.disaster:
            return

        for button in self.buttons:
            if button["rect"].collidepoint(pos):
                if button["action"] == "next_round":
                    self.next_round()
                else:
                    if button["cost"] == 0 or self.can_afford(button["cost"]):
                        self.selected_mode = button["action"]
                return

        if GRID_OFFSET_X <= pos[0] < GRID_OFFSET_X + GRID_SIZE * CELL_SIZE and \
                GRID_OFFSET_Y <= pos[1] < GRID_OFFSET_Y + GRID_SIZE * CELL_SIZE:
            col = (pos[0] - GRID_OFFSET_X) // CELL_SIZE
            row = (pos[1] - GRID_OFFSET_Y) // CELL_SIZE

            if self.selected_mode == "build" and self.grid[row][col] is None:
                if self.spend_coins(self.build_cost):
                    self.grid[row][col] = Factory(row, col)
                    self.selected_mode = None

            elif self.selected_mode == "upgrade" and self.grid[row][col] is not None:
                if self.spend_coins(self.upgrade_cost):
                    self.grid[row][col].upgrade()
                    self.selected_mode = None

            elif self.selected_mode == "insure" and self.grid[row][col] is not None:
                if self.spend_coins(self.insure_cost):
                    self.grid[row][col].insure()
                    self.selected_mode = None

    def next_round(self):
        if self.disaster:
            return

        # Выбираем случайное стихийное бедствие
        disasters = ["tornado", "meteor", "flood"]
        disaster_type = random.choice(disasters)
        self.disaster = Disaster(disaster_type)
        self.show_message(f"Наступает {self.get_disaster_name(disaster_type)}!")

    def get_disaster_name(self, disaster_type):
        names = {
            "tornado": "Торнадо",
            "meteor": "Метеоритный дождь",
            "flood": "Наводнение"
        }
        return names.get(disaster_type, "Бедствие")

    def apply_disaster_damage(self):
        if not self.disaster:
            return

        affected = set()

        for row, col in self.disaster.affected_cells:
            if 0 <= row < GRID_SIZE and 0 <= col < GRID_SIZE:
                if self.grid[row][col] is not None:
                    if self.grid[row][col].take_damage():
                        self.grid[row][col] = None
                    affected.add((row, col))

        # Начисление дохода
        income = 0
        for row in range(GRID_SIZE):
            for col in range(GRID_SIZE):
                if self.grid[row][col] is not None:
                    income += self.grid[row][col].get_income()

        if income > 0:
            self.add_coins(income)

        self.disaster = None

    def update(self):
        if self.disaster:
            if not self.disaster.update():
                self.apply_disaster_damage()

        if self.message_timer > 0:
            self.message_timer -= 1

    def draw(self):
        screen.fill(WATER)

        # Песчаный берег
        pygame.draw.rect(screen, SAND, (0, GRID_OFFSET_Y + GRID_SIZE * CELL_SIZE + 20,
                                        SCREEN_WIDTH, 100))

        # Остров
        island_rect = pygame.Rect(GRID_OFFSET_X - 30, GRID_OFFSET_Y - 30,
                                  GRID_SIZE * CELL_SIZE + 60, GRID_SIZE * CELL_SIZE + 60)
        pygame.draw.ellipse(screen, GREEN, island_rect)

        # Сетка
        for row in range(GRID_SIZE):
            for col in range(GRID_SIZE):
                rect = pygame.Rect(GRID_OFFSET_X + col * CELL_SIZE,
                                   GRID_OFFSET_Y + row * CELL_SIZE,
                                   CELL_SIZE, CELL_SIZE)
                pygame.draw.rect(screen, (0, 0, 0, 50), rect, 1)

                if self.grid[row][col] is not None:
                    self.grid[row][col].draw()

        # Анимация бедствия
        if self.disaster:
            self.disaster.draw()

        # UI
        pygame.draw.rect(screen, GOLD, (20, 20, 200, 60), border_radius=10)
        pygame.draw.rect(screen, BLACK, (20, 20, 200, 60), 2, border_radius=10)
        coins_text = large_font.render(f"Монеты: {self.coins}", True, BLACK)
        screen.blit(coins_text, (30, 35))

        # Сообщение о текущем бедствии
        if self.disaster:
            disaster_text = font.render(f"{self.get_disaster_name(self.disaster.type)}!", True, RED)
            screen.blit(disaster_text, (SCREEN_WIDTH - 200, 30))

        if self.message_timer > 0:
            msg_surface = font.render(self.message, True, BLACK)
            msg_rect = msg_surface.get_rect(center=(SCREEN_WIDTH // 2, 100))
            pygame.draw.rect(screen, WHITE, msg_rect.inflate(20, 10), border_radius=5)
            pygame.draw.rect(screen, BLACK, msg_rect.inflate(20, 10), 2, border_radius=5)
            screen.blit(msg_surface, msg_rect)

        # Кнопки
        for button in self.buttons:
            color = button["color"]

            if button["cost"] > 0 and not self.can_afford(button["cost"]):
                color = tuple(max(50, c - 80) for c in button["color"])

            if self.selected_mode == button["action"]:
                color = tuple(min(255, c + 30) for c in button["color"])

            pygame.draw.rect(screen, color, button["rect"], border_radius=10)
            pygame.draw.rect(screen, BLACK, button["rect"], 2, border_radius=10)

            text_lines = button["text"].split()
            for i, line in enumerate(text_lines):
                text = button_font.render(line, True, BLACK)
                text_rect = text.get_rect(
                    centerx=button["rect"].centerx,
                    centery=button["rect"].centery - 10 + i * 20
                )
                screen.blit(text, text_rect)

            if button["cost"] > 0:
                cost_text = font.render(f"{button['cost']} монет", True, BLACK)
                cost_rect = cost_text.get_rect(
                    centerx=button["rect"].centerx,
                    bottom=button["rect"].bottom - 5
                )
                screen.blit(cost_text, cost_rect)


def main():
    game = Game()
    running = True

    while running:
        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                running = False
            elif event.type == pygame.MOUSEBUTTONDOWN:
                if event.button == 1:
                    game.handle_click(event.pos)

        game.update()
        game.draw()
        pygame.display.flip()
        clock.tick(60)

    pygame.quit()
    sys.exit()


if __name__ == "__main__":
    main()