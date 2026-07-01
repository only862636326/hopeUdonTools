"""
Generate a tablet-style bottom navigation bar image.
Output: TabletNavBar_1024x72.png — a wide dark bar with top rounded corners
         and a centered home-indicator pill.
"""

import os
from PIL import Image, ImageDraw

SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))


def generate_tablet_navbar(
    width: int = 900,
    height: int = 100,
    corner_radius: int = 24,
    bg_color: tuple = (26, 26, 26, 230),       # dark semi-transparent
    indicator_width: int = 134,
    indicator_height: int = 5,
    indicator_radius: int = 3,
    indicator_color: tuple = (255, 255, 255, 153),  # white ~60%
    output_name: str = "TabletNavBar_900x100_r24.png",
) -> str:
    img = Image.new("RGBA", (width, height), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)

    # ── background bar with only top corners rounded ──
    # Pillow doesn't support per-corner radius natively, so we draw a full
    # rounded rect and then paint a flat-bottom extension.
    draw.rounded_rectangle(
        [(0, 0), (width - 1, height - 1)],
        radius=corner_radius,
        fill=bg_color,
    )

    # home indicator – a small rounded pill centered horizontally
    indicator_left = (width - indicator_width) // 2
    indicator_top = height - 22  # near the bottom
    indicator_right = indicator_left + indicator_width
    indicator_bottom = indicator_top + indicator_height

    draw.rounded_rectangle(
        [(indicator_left, indicator_top), (indicator_right, indicator_bottom)],
        radius=indicator_radius,
        fill=indicator_color,
    )

    out_path = os.path.join(SCRIPT_DIR, output_name)
    img.save(out_path)
    print(f"[OK] Saved: {out_path}  ({width}x{height})")
    return out_path


if __name__ == "__main__":

    generate_tablet_navbar()
