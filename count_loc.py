from __future__ import annotations

from pathlib import Path
from typing import Iterable


ROOT = Path(__file__).resolve().parent
SRC_DIR = ROOT / "src"


def strip_comments(line: str, in_block_comment: bool) -> tuple[str, bool]:
    result: list[str] = []
    i = 0
    in_string = False
    string_char = ""

    while i < len(line):
        char = line[i]
        next_char = line[i + 1] if i + 1 < len(line) else ""

        if in_block_comment:
            if char == "*" and next_char == "/":
                in_block_comment = False
                i += 2
                continue

            i += 1
            continue

        if in_string:
            result.append(char)

            if char == "\\" and i + 1 < len(line):
                result.append(line[i + 1])
                i += 2
                continue

            if char == string_char:
                in_string = False
                string_char = ""

            i += 1
            continue

        if char in {'"', "'"}:
            in_string = True
            string_char = char
            result.append(char)
            i += 1
            continue

        if char == "/" and next_char == "/":
            break

        if char == "/" and next_char == "*":
            in_block_comment = True
            i += 2
            continue

        result.append(char)
        i += 1

    return "".join(result), in_block_comment


def count_file(path: Path) -> tuple[int, int, int]:
    total = 0
    non_empty = 0
    non_comment = 0
    in_block_comment = False

    for line in path.read_text(encoding="utf-8").splitlines():
        total += 1

        if line.strip():
            non_empty += 1

        stripped_line, in_block_comment = strip_comments(line, in_block_comment)
        if stripped_line.strip():
            non_comment += 1

    return total, non_empty, non_comment


def main() -> None:
    if not SRC_DIR.exists():
        raise SystemExit(f"Directory not found: {SRC_DIR}")

    total = 0
    non_empty = 0
    non_comment = 0
    per_project: dict[str, tuple[int, int, int]] = {}

    for path in sorted(SRC_DIR.rglob("*.cs")):
        file_total, file_non_empty, file_non_comment = count_file(path)
        total += file_total
        non_empty += file_non_empty
        non_comment += file_non_comment

        project_name = path.relative_to(SRC_DIR).parts[0]
        project_total, project_non_empty, project_non_comment = per_project.get(
            project_name,
            (0, 0, 0),
        )
        per_project[project_name] = (
            project_total + file_total,
            project_non_empty + file_non_empty,
            project_non_comment + file_non_comment,
        )

    print_table(per_project.items())
    print()
    print(f"Total Lines of Code: {total}")
    print(f"Total Lines of Code without empty lines: {non_empty}")
    print(
        "Total Lines of Code without empty lines and without comments: "
        f"{non_comment}"
    )


def print_table(rows: Iterable[tuple[str, tuple[int, int, int]]]) -> None:
    header_project = "Project"
    header_total = "Total"
    header_non_empty = "No Empty"
    header_non_comment = "No Empty/Comments"

    normalized_rows = [
        (project, counts[0], counts[1], counts[2])
        for project, counts in sorted(rows, key=lambda item: item[0].lower())
    ]

    project_width = max(len(header_project), *(len(row[0]) for row in normalized_rows))
    total_width = max(len(header_total), *(len(str(row[1])) for row in normalized_rows))
    non_empty_width = max(len(header_non_empty), *(len(str(row[2])) for row in normalized_rows))
    non_comment_width = max(
        len(header_non_comment),
        *(len(str(row[3])) for row in normalized_rows),
    )

    separator = (
        f"+-{'-' * project_width}-+-{'-' * total_width}-"
        f"+-{'-' * non_empty_width}-+-{'-' * non_comment_width}-+"
    )

    print(separator)
    print(
        f"| {header_project:<{project_width}} | {header_total:>{total_width}} | "
        f"{header_non_empty:>{non_empty_width}} | {header_non_comment:>{non_comment_width}} |"
    )
    print(separator)

    for project, row_total, row_non_empty, row_non_comment in normalized_rows:
        print(
            f"| {project:<{project_width}} | {row_total:>{total_width}} | "
            f"{row_non_empty:>{non_empty_width}} | {row_non_comment:>{non_comment_width}} |"
        )

    print(separator)


if __name__ == "__main__":
    main()
