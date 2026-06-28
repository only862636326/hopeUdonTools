"""
Rounded rectangle image generator.
Generates filled (mask) or outlined (frame) rounded-rectangle PNGs.
Images are saved into the same directory as this script.
Depends on: pip install Pillow
"""

import os
import argparse
from PIL import Image, ImageDraw

SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))


def generate_rounded_rect(
    width: int = 256,
    height: int = 256,
    radius: int = 40,
    mode: str = "mask",
    stroke_width: int = 4,
    fill_color: tuple = (255, 255, 255, 255),
    bg_color: tuple = (0, 0, 0, 0),
    invert: bool = False,
    output_path: str = None,
) -> Image.Image:
    """
    Generate a rounded-rectangle image.

    Args:
        width:         Image width in pixels.
        height:        Image height in pixels.
        radius:        Corner radius in pixels.
        mode:          "mask" (filled) | "frame" (outline/stroke).
        stroke_width:  Outline stroke width in pixels (frame mode only).
        fill_color:    Foreground RGBA tuple, default opaque white.
        bg_color:      Background RGBA tuple, default fully transparent.
        invert:        When True, swaps opaque and transparent areas
                       (white becomes transparent, transparent becomes white).
        output_path:   Optional save path (relative paths resolve to script dir).

    Returns:
        PIL.Image object.
    """
    radius = max(0, min(radius, min(width, height) // 2))

    img = Image.new("RGBA", (width, height), bg_color)
    draw = ImageDraw.Draw(img)

    if mode == "mask":
        draw.rounded_rectangle(
            [(0, 0), (width - 1, height - 1)],
            radius=radius,
            fill=fill_color,
        )
    elif mode == "frame":
        draw.rounded_rectangle(
            [(stroke_width // 2, stroke_width // 2),
             (width - 1 - stroke_width // 2, height - 1 - stroke_width // 2)],
            radius=radius,
            outline=fill_color,
            width=stroke_width,
        )
    else:
        raise ValueError(f"Unknown mode: {mode}. Use 'mask' or 'frame'.")

    if invert:
        # Swap opaque <-> transparent: original white becomes transparent,
        # original transparent becomes opaque white.
        alpha = img.split()[3]
        inverted_alpha = alpha.point(lambda p: 255 - p)
        white_bg = Image.new("RGBA", (width, height), (255, 255, 255, 255))
        white_bg.putalpha(inverted_alpha)
        img = white_bg

    if output_path:
        if not os.path.isabs(output_path):
            output_path = os.path.join(SCRIPT_DIR, output_path)
        os.makedirs(os.path.dirname(output_path) or ".", exist_ok=True)
        img.save(output_path)
        print(f"[OK] Saved: {output_path}  ({width}x{height}, radius={radius}, mode={mode}"
              f"{', inverted' if invert else ''})")

    return img


def batch_generate(configs: list[dict]):
    """Batch-generate multiple images. Each dict is a keyword-arg set for generate_rounded_rect."""
    for cfg in configs:
        generate_rounded_rect(**cfg)


# ── CLI entry point ────────────────────────────────────────
if __name__ == "__main__":
    
    width = 640
    height = 360
    radius = 30
    mode = "mask"
    stroke_width = 4
    invert = True

    parser = argparse.ArgumentParser(description="Generate a rounded-rectangle image")
    parser.add_argument("--width", type=int, default=width, help="Image width (px)")
    parser.add_argument("--height", type=int, default=height, help="Image height (px)")
    parser.add_argument("--radius", type=int, default=radius, help="Corner radius (px)")
    parser.add_argument("--mode", choices=["mask", "frame"], default=mode,
                        help="mask=filled | frame=outline")
    
    parser.add_argument("--stroke", type=int, default=stroke_width, help="Stroke width for frame mode (px)")
    parser.add_argument("--invert", action="store_true", default=invert,
                        help="Invert: white becomes transparent, transparent becomes white")
    parser.add_argument("--output", type=str, default=None,
                        help="Output filename (saved next to this script; defaults to auto-named)")
    args = parser.parse_args()

    suffix = f"r{args.radius}" if args.mode == "mask" else f"r{args.radius}s{args.stroke}"
    if args.invert:
        suffix += "_inv"
    if args.output is None:
        args.output = f"Rounded{args.mode.capitalize()}_{args.width}x{args.height}_{suffix}.png"

    generate_rounded_rect(
        width=args.width,
        height=args.height,
        radius=args.radius,
        mode=args.mode,
        stroke_width=args.stroke,
        invert=args.invert,
        output_path=args.output,
    )
