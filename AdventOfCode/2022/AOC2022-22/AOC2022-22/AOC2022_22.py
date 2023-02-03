
from cmath import nan, pi, polar, rect

RIGHT, DOWN, LEFT, UP = 0, 1, 2, 3
DIRECTION_DELTAS = {RIGHT: 1, DOWN: 1j, LEFT: -1, UP: -1j}


class C(complex):
    """Bastardisation of complex native that snaps to grid."""

    def __new__(cls, c: complex) -> "C":
        return super().__new__(cls, round(c.real) + round(c.imag) * 1j)

    def __add__(self, other: complex) -> "C":
        return C(super().__add__(other))

    def __sub__(self, other: complex) -> "C":
        return C(super().__sub__(other))

    def __mul__(self, other: int) -> "C":
        return C(super().__mul__(other))

    def __floordiv__(self, divisor: int) -> "C":
        return C(self.real // divisor + (self.imag // divisor) * 1j)

    @property
    def real(self) -> int:
        return int(super().real)

    @property
    def imag(self) -> int:
        return int(super().imag)


class Cube:
    size: int
    grid: list[list[str]]
    my_loc: C
    my_dir: int
    my_face : int

    def __init__(self, input: list[str]) -> None:
        self.grid = []
        for row in input:
            self.grid.append(list(row))
        self.size = 50  # Lazily hardcoding size; could infer from input.
        self.my_loc = C(self.grid[0].index("."))
        self.my_dir = RIGHT
        self.my_face = 1

    def is_point_on_grid(self, p: C) -> bool:
        if p.imag < 0 or p.real < 0:
            return False
        try:
            return self.grid[p.imag][p.real] != " "
        except IndexError:
            return False

    def find_connecting_face(
        self, cur: C, want: int, dir: int, skip: set[C] = set()
    ) -> tuple[C, int] | None:
        """
        TRAVERSAL
        CUR: Top left point of face I'm currently on.
        WANT: Direction of target face.
        DIR: Direction I'll be entering target face from if I move into it now.
        SKIP: Faces I've already traversed.
        RULES:
        PREV WANT | A  A  A OP
        PREV DIR  | B  B  B  B
        ----------+-----------
        MOVE      | A RA !A  C
        ----------+-----------
        NEW WANT  | -  A OP  C
        NEW DIR   | B RB  B B?
        (B? = B if we moved in same direction as last turn, else !B)
        """
        res = None
        for move, delta in DIRECTION_DELTAS.items():
            if not self.is_point_on_grid(next := cur + delta * self.size):
                continue
            if next in skip:
                continue
            if move == want:
                return next, dir
            elif move == (want - 1) % 4:
                res = res or self.find_connecting_face(
                    cur=next, want=want, dir=(dir - 1) % 4, skip=skip | {cur}
                )
            elif move == (want + 1) % 4:
                res = res or self.find_connecting_face(
                    cur=next, want=want, dir=(dir + 1) % 4, skip=skip | {cur}
                )
            elif move == (want + 2) % 4:
                # In this situation, we want to keep track of the move we've just made.
                # Let's just be gross and encode it into `dir`.
                # Also note we're using 'nan' to denote "opposite side".
                res = res or self.find_connecting_face(
                    cur=next, want=nan, dir=dir + 4 * move, skip=skip | {cur}
                )
            elif want is nan:
                last_move = dir // 4
                dir = dir % 4
                res = res or self.find_connecting_face(
                    cur=next,
                    want=move,
                    dir=dir if move == last_move else (dir + 2) % 4,
                    skip=skip | {cur},
                )
        return res

    def move_forward(self, debugfile,n: int,instructionCount:int, wrap: str) -> None:
        for _ in range(n):
            if wrap == "flat":
                loc = self.my_loc
                while True:
                    loc += DIRECTION_DELTAS[self.my_dir]
                    # Check if we need to wrap.
                    if loc.imag >= len(self.grid):
                        loc = C(loc.real)
                    elif loc.imag < 0:
                        loc = C(loc.real + (len(self.grid) - 1) * 1j)
                    elif loc.real >= len(self.grid[loc.imag]):
                        loc = C(loc.imag * 1j)
                    elif loc.real < 0:
                        loc = C((len(self.grid[loc.imag]) - 1) + loc.imag * 1j)
                    match self.grid[loc.imag][loc.real]:
                        case " ":  # Keep 'wrapping'.
                            continue
                        case "." | ">" | "v" | "<" | "^":  # Successful movement.
                            self.grid[loc.imag][loc.real] = ">v<^"[self.my_dir]
                            break
                        case "#":  # Wall. No more movement will occur.
                            return
                self.my_loc = loc
            elif wrap == "3D":
                loc = self.my_loc + DIRECTION_DELTAS[self.my_dir]
                dir = self.my_dir
                if not self.is_point_on_grid(loc):
                    s = self.size
                    # Wrapping time! Find connecting face and entry dir to apply transform to loc & dir.
                    f, edir = self.find_connecting_face(
                        cur=self.my_loc // s * s, want=dir, dir=dir
                    )
                    self.my_face = edir
                    pf = loc // s * s  # the "phantom" face that loc is on
                    # Shift by delta between phantom face and target face.
                    loc += f - pf
                    # Rotate until dir matches target entry dir.
                    # NOTE: Cast to complex here, to allow decimal x/y vals when offsetting for polar.
                    # (our "C" class snaps everything to the grid!)
                    while dir != edir:
                        dir = (dir + 1) % 4
                        offset = complex(f) + (s - 1) / 2 + ((s - 1) / 2) * 1j
                        rho, phi = polar(complex(loc) - offset)
                        loc = C(rect(rho, phi + pi / 2) + offset)
                match self.grid[loc.imag][loc.real]:
                    case "#":  # Wall. No more movement will occur.
                        debugfile.write("Instuction "+str(instructionCount)+" position is "+str(self.my_loc)+" face is "+str(self.my_face)+"\n")

                        return
                    case "." | ">" | "v" | "<" | "^":  # Successful movement.
                        self.grid[loc.imag][loc.real] = ">v<^"[dir]
                        self.my_loc = loc
                        self.my_dir = dir
                        debugfile.write("Instuction "+str(instructionCount)+" position is "+str(self.my_loc)+" face is "+str(self.my_face)+"\n")

    def turn_right(self) -> None:
        self.my_dir = (self.my_dir + 1) % 4

    def turn_left(self) -> None:
        self.my_dir = (self.my_dir - 1) % 4

    def follow_instructions(self, debugfile ,instructions: str, wrap: str) -> None:

        instructionCount = 0

        while instructions:
            truncate = 1
            if instructions[0] == "L":
                cube.turn_left()
            elif instructions[0] == "R":
                cube.turn_right()
            else:
                truncate = (
                    x.index("R")
                    if "R" in (x := instructions.replace("L", "R"))
                    else len(instructions)
                )
                n_steps = instructions[:truncate]
                print(str(instructionCount)+"    moving forward "+n_steps+" facing "+str((cube.my_dir*90)))
                cube.move_forward(debugfile,int(n_steps),instructionCount, wrap=wrap)
            instructions = instructions[truncate:]
            #debugfile.write("Instuction "+str(instructionCount)+" position is "+str(cube.my_loc)+"\n")
            #if instructionCount > 1000 :
            #        break;
            instructionCount = instructionCount+1


    def draw(self):
        for row in self.grid:
            print("".join(row))

    def write(self):
        f = open("D:/GitHub/xexuxjy/AdventOfCode/2022/Solutions/Data/python22-draw.txt","w")
        for row in self.grid:
            f.write("".join(row))
            f.write("\n")

        f.close()

    def utter_password(self) -> int:
        return 1000 * (self.my_loc.imag + 1) + 4 * (self.my_loc.real + 1) + self.my_dir


data = open("D:/GitHub/xexuxjy/AdventOfCode/2022/Solutions/Data/puzzle-22-input.txt").read()
cube_data, instructions = data.split("\n\n")


debugOutput = open("D:/GitHub/xexuxjy/AdventOfCode/2022/Solutions/Data/puzzle-22-python-debug.txt","w")

#cube = Cube(cube_data.splitlines())
#cube.follow_instructions(debugOutput,instructions, wrap="flat")
#cube.draw()
#cube.write()
#print(f"Part 1: {cube.utter_password()}")

cube = Cube(cube_data.splitlines())
cube.follow_instructions(debugOutput,instructions, wrap="3D")
#cube.draw()
print(f"Part 2: {cube.utter_password()}")

debugOutput.close()