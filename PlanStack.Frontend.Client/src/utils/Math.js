const GRID_SIZE = 20;
const WALL_THICKNESS = 5;
const ERASE_TOLERANCE = 3;

// snap coordinates to grid
export const snapToGrid = (coord) => Math.round(coord / GRID_SIZE) * GRID_SIZE;

export const isPointNearWall = (px, py, wall) => {
  const { startX, startY, endX, endY } = wall;

  const l2 = (endX - startX) ** 2 + (endY - startY) ** 2;
  if (l2 === 0) return false; // Not a segment

  let t =
    ((px - startX) * (endX - startX) + (py - startY) * (endY - startY)) / l2;
  t = Math.max(0, Math.min(1, t));
  const closestX = startX + t * (endX - startX);
  const closestY = startY + t * (endY - startY);
  const distance = Math.sqrt((px - closestX) ** 2 + (py - closestY) ** 2);

  return distance <= WALL_THICKNESS + ERASE_TOLERANCE;
};

export const floodFill = (
  grid,
  startX,
  startY,
  newValue,
  targetValue,
  width,
  height
) => {
  if (grid[startY][startX] !== targetValue) {
    return { squareMeters: 0 };
  }

  let squareMeters = 0;
  let minX = Infinity,
    maxX = -Infinity,
    minY = Infinity,
    maxY = -Infinity;
  const allCells = [];
  const queue = [[startX, startY]];
  grid[startY][startX] = newValue;

  while (queue.length > 0) {
    const [x, y] = queue.shift();

    // checking if its a cell center
    if (x % 2 === 1 && y % 2 === 1) {
      squareMeters++;
      allCells.push([x, y]);

      // Update bounding box
      if (x < minX) minX = x;
      if (x > maxX) maxX = x;
      if (y < minY) minY = y;
      if (y > maxY) maxY = y;
    }

    // checking neighbors
    const neighbors = [
      [x + 1, y],
      [x - 1, y],
      [x, y + 1],
      [x, y - 1],
      [x + 1, y + 1],
      [x - 1, y - 1],
      [x + 1, y - 1],
      [x - 1, y + 1],
    ];

    for (const [nx, ny] of neighbors) {
      // checking bounds and target value
      if (
        nx >= 0 &&
        nx < width &&
        ny >= 0 &&
        ny < height &&
        grid[ny][nx] === targetValue
      ) {
        grid[ny][nx] = newValue;
        queue.push([nx, ny]);
      }
    }
  }
  return { squareMeters, minX, maxX, minY, maxY, allCells };
};
