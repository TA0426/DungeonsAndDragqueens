const api = {
	token: sessionStorage.getItem("dd-token"),
	userName: sessionStorage.getItem("dd-user"),
	isRegistering: false,
	characters: [],
	monsters: [],
	dungeons: [],
	swords: [],
	armors: [],
	inventory: [],
	features: null,
	selectedCharacterId: sessionStorage.getItem("dd-character"),
	selectedMonsterId: sessionStorage.getItem("dd-monster"),
	selectedDungeonId: sessionStorage.getItem("dd-dungeon"),
};

const characterArt = {
	Warrior:
		"  \\  |  /\n---\\*/---\n   /#\\\n  (o.o)\n /|###|\\\n   / \\",
	Wizard:
		"    /\\\n   /  \\\n  /_**_\\\n    \\o/\n   (o.o)\n   /|\\\n   / \\",
	Warlock: "  \\  ^  /\n   \\ | /\n  (o.o)\n  /|###|\\\n   /| |\\",
	Ranger:
		"   .---.\n  / o o \\\n /   ^   \\\n(  (___)  )\n   /|>---\n  / | \\",
	Assassin:
		"  /\\   /\\\n /  \\_/  \\\n |  -.-  |\n  \\ _=_ /\n   /| |\\\n  / | | \\",
};

const monsterArt = {
	slime:
		"   .----.\n  / .--. \\\n | ( oo ) |\n  \\ '--' /\n   '----'",
	goblin:
		"  .-^^^^-.\n /  o  o  \\\n|   > <   |\n \\  ===  /\n  /|###|\\\n   /   \\",
	wraith:
		"      .-.\n    _(   )_\n   /  o o  \\\n  |    ^    |\n   \\  ___  /\n    '-----'",
	ghost:
		"   .----.\n  / .--. \\\n | ( o  o ) |\n |  \\__/  |\n  \\_    _/\n    /__\\",
	ghoul:
		"   .-----.\n  /  o o  \\\n |  /___\\  |\n |  \\___/  |\n  \\_|||_/\n   /   \\",
	undead:
		"   .----.\n  / x  x \\\n |   .-.   |\n  \\  === /\n  /|###|\\\n   /   \\",
	skeleton:
		"   .----.\n  / o  o \\\n |   .-.   |\n  \\  =  /\n  _/|###|\\_\n    /   \\",
	zombie:
		"  .-~~~-.\n /  x  o \\\n|   .---.  |\n \\  ~~~  /\n  /|###|\\\n  /  _  \\",
	vampire:
		"  .-^^^-.\n /  o o  \\\n|   \\_/   |\n \\  /V\\  /\n  \\|###|/\n   /   \\",
	lich: "   .-.-.\n  /  x x \\\n |  .---.  |\n  \\  === /\n  /|#####|\\\n    /   \\",
	orc: "  .-^^^^-.\n /  o  o  \\\n|  >====<  |\n \\  ===  /\n /|#####|\\\n   /   \\",
	ogre: "  .------.\n /  o  o  \\\n|   (___)  |\n \\  ===  /\n  /|####|\\\n  /      \\",
	troll:
		"  .------.\n /  @  @  \\\n|   .---.  |\n \\  ===  /\n  /|####|\\\n / |    | \\",
	golem:
		"   .----.\n  / [] [] \\\n |  |####|  |\n |  | ## |  |\n  \\_|##|_/\n    /  \\",
	wizard:
		"     /\\\n    /++\\\n   /____\\\n  ( o  o )\n   \\_==_/\n   /|###|\\\n    /   \\",
	mage: "     /\\\n    /**\\\n   /____\\\n  ( o  o )\n   \\_==_/\n    /|\\",
	warlock: "  \\  ^  /\n   \\ | /\n  ( o.o )\n  /|###|\\\n   / | \\",
	witch:
		"   .-^^-.\n  /  o o \\\n |   \\_/  |\n  \\  === /\n  /|###|\\\n   /   \\",
	dragon:
		"      /\\_/\\\n  ___/  o o  \\___\n /   \\  ^  /    \\\n|  ___/====\\___  |\n \\_/  /####\\  \\_/\n      /_/  \\_\\",
	demon:
		"  \\  ^  /\n   \\ | /\n  / .-. \\\n | ( x x ) |\n  \\  ^  /\n  /|###|\\",
	imp: "  \\ ^ ^ /\n   \\o.o/\n   (  ^ )\n    /|\\\n   _/ \\_",
	"fire elemental":
		"    .  *  .\n   \\  ^  /\n  (  o.o  )\n  /  ###  \\\n    /   \\",
	"ice elemental":
		"   * . . *\n  /  o o  \\\n |  .---.  |\n  \\  === /\n   /|###|\\\n    /   \\",
	mummy:
		"   .----.\n  / /////\\\n |  o  o  |\n | /====\\ |\n  \\_||||_/\n   /   \\",
	cyclops:
		"  .------.\n /        \\\n|    (o)   |\n \\   ___  /\n  /|#####|\\\n    /   \\",
	minotaur:
		" \\  ^  ^  /\n  \\ |  | /\n  / o  o \\\n |   (__)  |\n  \\ ##### /\n   /|   |\\",
	hydra:
		" /\\  /\\  /\\\n( o )( o )( o )\n \\__/\\__/\\__/\n    |####|\n    /    \\",
	kraken:
		"  .-~~~~-.\n /  o  o  \\\n|   (___)  |\n \\_/|###|\\_/\n  ~ /| |\\ ~\n   / / \\ \\",
	gargoyle:
		"  /\\  /\\\n /  \\__/  \\\n|  o  o  |\n \\  === /\n /|#####|\\\n/ /     \\ \\",
	phoenix:
		"   \\ | /\n  -- * --\n  ( o.o )\n /|#####|\\\n/  /   \\  \\\n  /     \\",
	elemental:
		"    . * .\n   / o o \\\n  (   ^   )\n   \\ ### /\n    /   \\",
	spider:
		" /\\  |  /\\\n(  o\\ | /o  )\n \\  (o.o)  /\n /\\ /###\\ /\\\n    / | \\",
	werewolf: " /\\_/\\\n(  o o  )\n \\  ^  /\n /|###|\\\n/ |   | \\",
	wolf: " /\\_/\\\n(  o o  )\n \\  ^  /\n  /| |\\\n / | | \\",
	bat: " /\\   /\\\n(  \\_/  )\n \\ (o.o) /\n  \\ ### /\n   /   \\",
	serpent: "   /^\\\n  /o o\\\n /  ^  \\\n(  /###\n \\_/  \\",
	snake: "   /^\\\n  /o o\\\n (  ^  )___\n  \\###  __)\n   \\___/",
	knight:
		"   .----.\n  / [] [] \\\n |   .--.   |\n  \\  ===  /\n  /|#####|\\\n   /  |  \\",
	mimic:
		"   .-----.\n  /  o o  \\\n |  /^^^^\\  |\n |  \\____/  |\n  \\_||||_/\n   /_____\\",
	harpy: "  \\  ^  /\n   \\o.o/\n  /|###|\\\n / /| |\\ \\\n   /   \\",
	beast:
		"  .-^^^^-.\n /  o  o  \\\n|   ( ^ )  |\n \\  ===  /\n  /|###|\\\n   /   \\",
	monster:
		"  .-^^^^-.\n /  o  o  \\\n|   (___)  |\n \\  ===  /\n  /|###|\\\n   /   \\",
};

function getMonsterArt(monster) {
	const monsterType = monster.typeOfMonster.trim().toLowerCase();
	const broadTypes = ["undead", "beast", "elemental", "monster"];
	if (monsterArt[monsterType] && !broadTypes.includes(monsterType))
		return monsterArt[monsterType];

	const name = monster.name.toLowerCase();
	const namedType = Object.keys(monsterArt)
		.sort((first, second) => second.length - first.length)
		.find((candidate) => new RegExp(`\\b${candidate}\\b`).test(name));
	return (
		monsterArt[namedType] ||
		monsterArt[monsterType] ||
		monsterArt.monster
	);
}

const byId = (id) => document.getElementById(id);

function renderCharacterClassOptions() {
	const options = byId("character-class-options");
	for (const [name, art] of Object.entries(characterArt)) {
		const label = document.createElement("label");
		label.className = "class-option";
		const radio = document.createElement("input");
		radio.type = "radio";
		radio.name = "character-class";
		radio.value = name;
		radio.checked = name === "Warrior";
		radio.required = true;
		const preview = document.createElement("pre");
		preview.textContent = art;
		const title = document.createElement("span");
		title.textContent = name;
		label.append(radio, preview, title);
		options.append(label);
	}
}

renderCharacterClassOptions();

function setStatus(text, online = true) {
	byId("connection-status").textContent = text;
	document.querySelector(".status-dot").style.background = online
		? "var(--green)"
		: "var(--red)";
}

function showMessage(message, error = false) {
	const output = byId("game-message");
	output.textContent = message;
	output.style.color = error ? "var(--red)" : "var(--orange)";
}

function updateBattleHp(
	characterHp,
	characterMaxHp,
	monsterHp,
	monsterMaxHp,
) {
	for (const [id, hp, maxHp] of [
		["character", characterHp, characterMaxHp],
		["monster", monsterHp, monsterMaxHp],
	]) {
		const maximum = Math.max(1, maxHp);
		const current = Math.max(0, Math.min(hp, maximum));
		const percent = (current / maximum) * 100;
		byId(`${id}-battle-hp-value`).textContent =
			`${current} / ${maximum} HP`;
		byId(`${id}-battle-hp-fill`).style.width = `${percent}%`;
		const track = byId(`${id}-battle-hp-track`);
		track.setAttribute("aria-valuemax", maximum);
		track.setAttribute("aria-valuenow", current);
	}
}

function setLoggedIn(token, userName) {
	api.token = token;
	api.userName = userName;
	sessionStorage.setItem("dd-token", token);
	sessionStorage.setItem("dd-user", userName);
	byId("auth-screen").hidden = true;
	byId("game-screen").hidden = false;
	byId("logout-button").hidden = false;
	byId("player-name").textContent = userName;
	setStatus("CONNECTED");
	refreshGame();
}

function logOut() {
	sessionStorage.removeItem("dd-token");
	sessionStorage.removeItem("dd-user");
	sessionStorage.removeItem("dd-character");
	api.token = null;
	api.userName = null;
	api.selectedCharacterId = null;
	byId("game-screen").hidden = true;
	byId("auth-screen").hidden = false;
	byId("logout-button").hidden = true;
	byId("auth-error").textContent = "";
	setStatus("SYSTEM READY");
}

async function request(path, options = {}) {
	const headers = new Headers(options.headers || {});
	if (options.body) headers.set("Content-Type", "application/json");
	if (api.token) headers.set("Authorization", `Bearer ${api.token}`);

	const response = await fetch(path, { ...options, headers });
	if (response.status === 401 && api.token) {
		logOut();
		throw new Error("Your session expired. Please log in again.");
	}

	const contentType = response.headers.get("content-type") || "";
	const body = contentType.includes("application/json")
		? await response.json()
		: await response.text();
	if (!response.ok) {
		const message =
			typeof body === "string" ? body : body.error || body.title;
		throw new Error(
			message || `Request failed (${response.status}).`,
		);
	}
	return body;
}

byId("auth-form").addEventListener("submit", async (event) => {
	event.preventDefault();
	const error = byId("auth-error");
	error.textContent = "";
	const userName = byId("username").value.trim();
	const password = byId("password").value;
	const endpoint = api.isRegistering
		? "/auth/register"
		: "/auth/login";
	byId("auth-submit").disabled = true;
	try {
		const result = await request(endpoint, {
			method: "POST",
			body: JSON.stringify({ userName, password }),
		});
		setLoggedIn(result.token, result.userName);
	} catch (exception) {
		error.textContent = exception.message;
	} finally {
		byId("auth-submit").disabled = false;
	}
});

byId("switch-auth").addEventListener("click", () => {
	api.isRegistering = !api.isRegistering;
	byId("auth-title").textContent = api.isRegistering
		? "Create account"
		: "Log in";
	byId("auth-mode-label").textContent = api.isRegistering
		? "NEW PLAYER REGISTRATION"
		: "PLAYER ACCESS";
	byId("auth-submit").textContent = api.isRegistering
		? "CREATE ACCOUNT"
		: "ENTER THE DUNGEON";
	byId("switch-prompt").textContent = api.isRegistering
		? "Already have an account?"
		: "New around here?";
	byId("switch-auth").textContent = api.isRegistering
		? "Log in"
		: "Create an account";
	byId("password").autocomplete = api.isRegistering
		? "new-password"
		: "current-password";
	byId("password").minLength = api.isRegistering ? 5 : 1;
	byId("password-hint").hidden = !api.isRegistering;
	byId("auth-error").textContent = "";
});

byId("logout-button").addEventListener("click", logOut);

function addOption(select, value, label) {
	const option = document.createElement("option");
	option.value = value;
	option.textContent = label;
	select.append(option);
}

function renderCharacters() {
	const list = byId("character-list");
	const select = byId("character-select");
	list.replaceChildren();
	select.replaceChildren();
	byId("character-count").textContent = api.characters.length;

	if (!api.characters.length) {
		const empty = document.createElement("p");
		empty.className = "card-stats";
		empty.textContent = "No adventurers yet. Recruit one below.";
		list.append(empty);
		addOption(select, "", "Create a character first");
		api.selectedCharacterId = null;
		return;
	}

	if (
		!api.characters.some(
			(character) => character.id === api.selectedCharacterId,
		)
	)
		api.selectedCharacterId = api.characters[0].id;

	for (const character of api.characters) {
		addOption(
			select,
			character.id,
			`${character.name} · LV ${character.level}`,
		);
		const card = document.createElement("article");
		card.className = `character-card${character.id === api.selectedCharacterId ? " selected" : ""}`;
		const title = document.createElement("div");
		title.className = "card-title";
		const portrait = document.createElement("pre");
		portrait.className = "character-portrait";
		portrait.textContent =
			characterArt[character.characterClass] || characterArt.Warrior;
		portrait.setAttribute("aria-hidden", "true");
		const name = document.createElement("span");
		name.textContent = character.name;
		const level = document.createElement("small");
		level.textContent = `LV ${character.level} · ${character.isHardcore ? "HARDCORE" : "CASUAL"}`;
		title.append(name, level);
		const stats = document.createElement("div");
		stats.className = "card-stats";
		stats.textContent = `HP ${character.hp}  /  DMG ${character.damage}`;
		const track = document.createElement("div");
		track.className = "hp-track";
		const fill = document.createElement("div");
		fill.className = "hp-fill";
		fill.style.width = `${Math.max(0, Math.min(100, (character.hp / Math.max(1, character.level * 300)) * 100))}%`;
		track.append(fill);
		const details = document.createElement("div");
		details.className = "character-details";
		details.append(title, stats, track);
		const body = document.createElement("div");
		body.className = "character-body";
		body.append(portrait, details);
		card.append(body);
		const deleteButton = document.createElement("button");
		deleteButton.type = "button";
		deleteButton.className = "delete-character-button";
		deleteButton.textContent = "DELETE CHARACTER";
		deleteButton.setAttribute("aria-label", `Delete ${character.name}`);
		deleteButton.addEventListener("click", async (event) => {
			event.stopPropagation();
			if (window.prompt('Type exactly: er du sure? / are you sure?') !== "er du sure? / are you sure?") return;
			try {
				await request(`/StoreCharacters/${character.id}`, { method: "DELETE" });
				if (api.selectedCharacterId === character.id) {
					sessionStorage.removeItem("dd-character");
					api.selectedCharacterId = null;
				}
				await refreshGame();
				showMessage(`${character.name} was deleted.`);
			} catch (exception) {
				showMessage(exception.message, true);
			}
		});
		card.append(deleteButton);
		card.addEventListener("click", () =>
			selectCharacter(character.id),
		);
		list.append(card);
	}
	select.value = api.selectedCharacterId;
}

async function selectCharacter(id) {
	api.selectedCharacterId = id;
	sessionStorage.setItem("dd-character", id);
	try {
		[api.inventory, api.dungeons] = await Promise.all([
			request(`/StoreCharacters/${id}/inventory`),
			request(`/Dungeons?characterId=${id}`),
		]);
	} catch (exception) {
		api.inventory = [];
		showMessage(exception.message, true);
	}
	renderCharacters();
	renderDungeons();
	renderSwords();
	renderArmors();
}

function renderDungeons() {
	const list = byId("dungeon-list");
	const select = byId("dungeon-select");
	list.replaceChildren();
	select.replaceChildren();
	byId("dungeon-count").textContent = api.dungeons.length;
	if (
		!api.dungeons.some(
			(item) => item.id === api.selectedDungeonId && item.isUnlocked,
		)
	)
		api.selectedDungeonId =
			api.dungeons.find((item) => item.isUnlocked && !item.isCompleted)?.id ||
			api.dungeons.find((item) => item.isUnlocked)?.id ||
			null;
	for (const dungeon of api.dungeons) {
		const dungeonIndex = api.dungeons.findIndex((item) => item.id === dungeon.id);
		const previous = api.dungeons[dungeonIndex - 1];
		addOption(
			select,
			dungeon.id,
			`${dungeon.name}${!dungeon.isUnlocked ? " · LOCKED" : dungeon.isEndless ? ` · WAVE ${dungeon.currentWave}` : dungeon.isCompleted ? ` · CLEARED ×${dungeon.clearCount} · REPLAY` : ` · WAVE ${dungeon.currentWave}`}`,
		);
		const card = document.createElement("article");
		card.className = `dungeon-card${dungeon.id === api.selectedDungeonId ? " selected" : ""}${dungeon.isCompleted ? " cleared" : ""}${!dungeon.isUnlocked ? " locked" : ""}${dungeon.isEndless ? " endless" : ""}`;
		if (!dungeon.isUnlocked) select.lastElementChild.disabled = true;
		card.tabIndex = 0;
		const heading = document.createElement("div");
		heading.className = "card-title";
		const name = document.createElement("span");
		name.textContent = dungeon.name;
		const difficulty = document.createElement("small");
		difficulty.textContent = dungeon.difficulty.toUpperCase();
		if (dungeon.isCompleted) {
			const victory = document.createElement("span");
			victory.className = "dungeon-victory";
			victory.textContent = `🏆 CLEARED ×${dungeon.clearCount}`;
			heading.append(name, victory, difficulty);
		} else heading.append(name, difficulty);
		const art = document.createElement("pre");
		art.className = "dungeon-art";
		art.textContent = dungeon.asciiArt;
		const world = document.createElement("p");
		world.className = "dungeon-world";
		world.textContent = dungeon.world;
		const description = document.createElement("p");
		description.className = "dungeon-description";
		description.textContent = dungeon.description;
		const stats = document.createElement("p");
		stats.className = "card-stats";
		stats.textContent = !dungeon.isUnlocked
			? `Defeat ${previous?.name || "the previous dungeon"} to unlock.`
			: `${dungeon.isEndless ? "Endless" : `Recommended: LV ${dungeon.recommendedLevel} · Sword ${dungeon.recommendedSwordDamage}+`} · ${dungeon.isBossWave ? dungeon.isEndless ? "ENDLESS BOSS" : "BOSS WAVE 51" : dungeon.isEndless ? `WAVE ${dungeon.currentWave} · BOSS EVERY 10` : `WAVE ${dungeon.currentWave}/50`} · ${dungeon.clearCount} clear${dungeon.clearCount === 1 ? "" : "s"}`;
		card.append(art, heading, world, description, stats);
		const choose = () => {
			if (!dungeon.isUnlocked) {
				showMessage(`Defeat ${previous?.name || "the previous dungeon"} first.`, true);
				return;
			}
			api.selectedDungeonId = dungeon.id;
			sessionStorage.setItem("dd-dungeon", dungeon.id);
			select.value = dungeon.id;
			renderDungeons();
		};
		card.addEventListener("click", choose);
		card.addEventListener("keydown", (event) => {
			if (event.key === "Enter" || event.key === " ") choose();
		});
		list.append(card);
	}
	select.value = api.selectedDungeonId || "";
	select.onchange = () => {
		if (!api.dungeons.find((item) => item.id === select.value)?.isUnlocked) return;
		api.selectedDungeonId = select.value;
		sessionStorage.setItem("dd-dungeon", select.value);
		renderDungeons();
	};
}

function renderSwords() {
	const select = byId("sword-select");
	const equipped = byId("equipped-sword");
	select.replaceChildren();
	const character = api.characters.find(
		(item) => item.id === api.selectedCharacterId,
	);
	const swords = api.inventory.filter(
		(item) => item.type === "sword",
	);
	const equippedSword = swords.find(
		(item) =>
			item.swordId === character?.swordId &&
			item.level === character?.swordLevel,
	);
	equipped.textContent = equippedSword
		? `Equipped: ${equippedSword.name} · LV ${equippedSword.level} · +${equippedSword.damage} DMG`
		: "Equipped: none";
	if (!swords.length) {
		addOption(select, "", "No swords available");
		byId("equip-button").disabled = true;
	}
	for (const sword of swords)
		addOption(
			select,
			sword.id,
			`${sword.name} · LV ${sword.level} · x${sword.quantity} · +${sword.damage} DMG`,
		);
	if (swords.length) {
		select.value = equippedSword?.id || swords[0].id;
		byId("equip-button").disabled = false;
	}
	select.onchange = renderGearArt;
	renderGearArt();
}

function renderGearArt() {
	const character = api.characters.find(
		(item) => item.id === api.selectedCharacterId,
	);
	const root = byId("gear-art");
	root.replaceChildren();
	const sword =
		api.inventory.find(
			(item) => item.id === byId("sword-select").value,
		) ||
		api.inventory.find(
			(item) =>
				item.type === "sword" &&
				item.swordId === character?.swordId &&
				item.level === character?.swordLevel,
		);
	const selectedArmorIds = [
		"helmet",
		"chest",
		"gloves",
		"legs",
		"boots",
	].map(
		(slot) =>
			byId("armor-slots").querySelector(`[data-slot="${slot}"]`)
				?.value,
	);
	const items = [
		sword,
		...api.inventory.filter(
			(item) =>
				item.type === "armor" && selectedArmorIds.includes(item.id),
		),
	].filter(Boolean);
	for (const item of items) {
		const piece = document.createElement("div");
		piece.className = "gear-piece";
		const art = document.createElement("pre");
		art.textContent = item.asciiArt || "";
		const label = document.createElement("span");
		label.textContent =
			item.type === "armor"
				? `${item.name} LV ${item.level} · DEF ${item.defense}${item.swordDamageBonus ? ` · +${item.swordDamageBonus} sword` : ""}`
				: `${item.name} LV ${item.level} · +${item.damage} DMG`;
		piece.append(art, label);
		root.append(piece);
	}
}

function renderArmors() {
	const character = api.characters.find(
		(item) => item.id === api.selectedCharacterId,
	);
	const root = byId("armor-slots");
	root.replaceChildren();
	for (const slot of ["helmet", "chest", "gloves", "legs", "boots"]) {
		const label = document.createElement("label");
		label.textContent = slot[0].toUpperCase() + slot.slice(1);
		const row = document.createElement("div");
		row.className = "input-row";
		const select = document.createElement("select");
		select.dataset.slot = slot;
		const pieces = api.inventory.filter(
			(item) =>
				item.type === "armor" && item.slot.toLowerCase() === slot,
		);
		for (const piece of pieces)
			addOption(
				select,
				piece.id,
				`${piece.name} · LV ${piece.level} · x${piece.quantity} · DEF ${piece.defense}${piece.swordDamageBonus ? ` · +${piece.swordDamageBonus} sword` : ""}`,
			);
		const equippedId = character?.[`${slot}Id`];
		const equippedLevel = character?.[`${slot}Level`];
		const equippedPiece = pieces.find(
			(item) =>
				item.armorId === equippedId && item.level === equippedLevel,
		);
		select.value = equippedPiece?.id || pieces[0]?.id || "";
		const button = document.createElement("button");
		button.className = "add-button";
		button.type = "button";
		button.textContent = "↗";
		button.disabled = !character || !pieces.length;
		select.onchange = renderGearArt;
		button.onclick = async () => {
			try {
				const piece = api.inventory.find(
					(item) => item.id === select.value,
				);
				const result = await request(
					`/StoreCharacters/${character.id}/armor/${slot}/${piece.armorId}/${piece.level}`,
					{ method: "PUT" },
				);
				showMessage(
					`${result.armorName} equipped · defense ${result.defense}${result.swordDamageBonus ? ` · +${result.swordDamageBonus} sword damage` : ""}.`,
				);
				await refreshGame();
			} catch (exception) {
				showMessage(exception.message, true);
			}
		};
		row.append(select, button);
		root.append(label, row);
	}
	renderGearArt();
}

function renderFeatures() {
	if (!api.features) return;
	byId("gold-balance").textContent = `Gold: ${api.features.gold}`;
	byId("buy-auto-fight").textContent = api.features.hasAutoFight
		? "Auto-fight purchased"
		: `Buy auto-fight · ${api.features.autoFightCost} gold`;
	byId("buy-auto-camp").textContent = api.features.hasAutoCamp
		? "Auto-camp purchased"
		: `Buy auto-camp · ${api.features.autoCampCost} gold`;
	byId("buy-auto-fight").disabled = api.features.hasAutoFight;
	byId("buy-auto-camp").disabled = api.features.hasAutoCamp;
	byId("auto-fight").disabled = !api.features.hasAutoFight;
	byId("auto-camp").disabled = !api.features.hasAutoCamp;
}

async function refreshGame() {
	setStatus("SYNCING…");
	try {
		const [characters, monsters, features] = await Promise.all([
			request("/StoreCharacters"),
			request("/StoreMonsters?page=1&pageSize=100"),
			request("/auth/features"),
		]);
		api.characters = characters;
		api.monsters = monsters;
		api.features = features;
		if (
			!characters.some((item) => item.id === api.selectedCharacterId)
		)
			api.selectedCharacterId = characters[0]?.id || null;
		api.inventory = api.selectedCharacterId
			? await request(
					`/StoreCharacters/${api.selectedCharacterId}/inventory`,
				)
			: [];
		api.dungeons = await request(
			api.selectedCharacterId
				? `/Dungeons?characterId=${api.selectedCharacterId}`
				: "/Dungeons",
		);
		renderCharacters();
		renderDungeons();
		renderSwords();
		renderArmors();
		renderFeatures();
		setStatus("CONNECTED");
	} catch (exception) {
		setStatus("OFFLINE", false);
		showMessage(exception.message, true);
	}
}

byId("new-character-form").addEventListener(
	"submit",
	async (event) => {
		event.preventDefault();
		const input = byId("new-character-name");
		try {
			const character = await request("/StoreCharacters", {
				method: "POST",
				body: JSON.stringify({
					name: input.value.trim(),
					isHardcore:
						document.querySelector(
							'input[name="character-mode"]:checked',
						).value === "hardcore",
					characterClass: document.querySelector(
						'input[name="character-class"]:checked',
					).value,
				}),
			});
			input.value = "";
			api.characters.push(character);
			byId("recruit-panel").open = false;
			await selectCharacter(character.id);
			showMessage(
				`${character.name} joined in ${character.isHardcore ? "Hardcore" : "Casual"} mode.`,
			);
		} catch (exception) {
			showMessage(exception.message, true);
		}
	},
);

byId("character-select").addEventListener("change", (event) => {
	selectCharacter(event.target.value);
});

byId("fight-button").addEventListener("click", async () => {
	const characterId = byId("character-select").value;
	const dungeonId = byId("dungeon-select").value;
	const dungeon = api.dungeons.find((item) => item.id === dungeonId);
	if (!characterId || !dungeon || !dungeon.isUnlocked)
		return showMessage(
			"Create a character and select an unlocked dungeon first.",
			true,
		);
	const button = byId("fight-button");
	const autoFight = byId("auto-fight");
	const fighter = api.characters.find(
		(item) => item.id === characterId,
	);
	byId("battle-character-art").textContent =
		characterArt[fighter?.characterClass] || characterArt.Warrior;
	byId("battle-monster-art").textContent = "Choosing a random enemy…";
	updateBattleHp(fighter?.hp ?? 0, fighter?.maxHp ?? 1, 0, 1);
	byId("battle-stage").classList.add("fighting");
	button.disabled = true;
	try {
		do {
			const monsterArtElement = byId("battle-monster-art");
			monsterArtElement.classList.remove("dead");
			const result = await request("/Fight/dungeon", {
				method: "POST",
				body: JSON.stringify({ characterId, dungeonId }),
			});
			let displayedCharacterHp =
				api.characters.find((item) => item.id === characterId)?.hp ??
				result.characterHpAfterFight;
			let displayedCharacterMaxHp =
				api.characters.find((item) => item.id === characterId)
					?.maxHp ?? result.characterMaxHp;
			let displayedMonsterHp = result.monsterMaxHp;
			updateBattleHp(
				displayedCharacterHp,
				displayedCharacterMaxHp,
				displayedMonsterHp,
				result.monsterMaxHp,
			);
			const target = api.monsters.find(
				(item) => item.name === result.monsterName,
			);
			monsterArtElement.textContent =
				result.monsterArt || (target ? getMonsterArt(target) : "");
			const log = byId("battle-log");
			log.replaceChildren();
			for (const line of result.battleLog || []) {
				const hpLeft = line.match(/^(.+) has (\d+) HP left\.$/);
				if (hpLeft) {
					if (hpLeft[1] === result.characterName)
						displayedCharacterHp = Number(hpLeft[2]);
					if (hpLeft[1] === result.monsterName)
						displayedMonsterHp = Number(hpLeft[2]);
				}
				const maxHp = line.match(/^Max HP increased to (\d+)\.$/);
				if (maxHp) displayedCharacterMaxHp = Number(maxHp[1]);
				if (hpLeft || maxHp)
					updateBattleHp(
						displayedCharacterHp,
						displayedCharacterMaxHp,
						displayedMonsterHp,
						result.monsterMaxHp,
					);
				const entry = document.createElement("p");
				entry.textContent = `> ${line}`;
				if (/defeat|wins|victory|level up/i.test(line))
					entry.className = "log-win";
				if (/damage|died|defeated/i.test(line))
					entry.className = "log-danger";
				log.append(entry);
				log.scrollTop = log.scrollHeight;
				await new Promise((resolve) => setTimeout(resolve, 450));
			}
			if (result.characterWon) {
				monsterArtElement.textContent =
					monsterArtElement.textContent.replace(
						/([oOxX@])([ .]{0,3})([oOxX@])/g,
						"X X",
					);
				monsterArtElement.classList.add("dead");
				await new Promise((resolve) => setTimeout(resolve, 750));
			}
			const summary = document.createElement("p");
			summary.className = result.characterWon
				? "log-win"
				: "log-danger";
			summary.textContent = `> ${result.message || (result.characterWon ? "Victory." : "Defeat.")}`;
			log.append(summary);
			log.scrollTop = log.scrollHeight;
			showMessage(result.message || "Fight complete.");
			await refreshGame();
			if (result.lootChoices?.length) await chooseLoot(result);
			let character = api.characters.find(
				(item) => item.id === characterId,
			);
			if (
				byId("auto-camp").checked &&
				character &&
				character.hp <= character.maxHp / 2
			) {
				try {
					const campResult = await request(
						`/StoreCharacters/${characterId}/camp`,
						{ method: "POST" },
					);
					showMessage(`Auto-camp: ${campResult.message}`);
					await refreshGame();
					character = api.characters.find(
						(item) => item.id === characterId,
					);
				} catch (exception) {
					byId("auto-fight").checked = false;
					showMessage(
						`Auto-camp could not heal: ${exception.message}`,
						true,
					);
					break;
				}
			}
			if (
				autoFight.checked &&
				(!character || character.hp < character.maxHp / 2)
			) {
				autoFight.checked = false;
				showMessage("Auto-fight stopped: character HP is below 50%.");
				break;
			}
			if (
				!autoFight.checked ||
				!result.characterWon ||
				result.dungeonCompleted
			)
				break;
		} while (autoFight.checked);
	} catch (exception) {
		showMessage(exception.message, true);
	} finally {
		byId("battle-stage").classList.remove("fighting");
		button.disabled = false;
	}
});

byId("auto-fight").addEventListener("change", (event) => {
	if (event.target.checked) {
		const character = api.characters.find(
			(item) => item.id === byId("character-select").value,
		);
		if (character && character.hp < character.maxHp / 2) {
			event.target.checked = false;
			showMessage("Auto-fight requires at least 50% HP.", true);
		}
	}
});

byId("buy-auto-fight").addEventListener("click", () =>
	purchaseAutoFeature("auto-fight"),
);
byId("buy-auto-camp").addEventListener("click", () =>
	purchaseAutoFeature("auto-camp"),
);

async function purchaseAutoFeature(feature) {
	try {
		api.features = await request(
			`/auth/features/${feature}/purchase`,
			{
				method: "POST",
			},
		);
		renderFeatures();
		showMessage(
			`${feature === "auto-fight" ? "Auto-fight" : "Auto-camp"} unlocked.`,
		);
	} catch (exception) {
		showMessage(exception.message, true);
	}
}

byId("auto-camp").addEventListener("change", (event) => {
	if (event.target.checked)
		showMessage(
			"Auto-camp will heal after a victory when HP is 50% or lower.",
		);
});

byId("camp-button").addEventListener("click", async () => {
	const characterId = byId("character-select").value;
	if (!characterId)
		return showMessage("Choose a character first.", true);
	try {
		const result = await request(
			`/StoreCharacters/${characterId}/camp`,
			{ method: "POST" },
		);
		showMessage(result.message || `Rested. HP is now ${result.hp}.`);
		await refreshGame();
	} catch (exception) {
		showMessage(exception.message, true);
	}
});

byId("equip-button").addEventListener("click", async () => {
	const characterId = byId("character-select").value;
	const item = api.inventory.find(
		(gear) => gear.id === byId("sword-select").value,
	);
	if (!characterId || !item)
		return showMessage("Choose a character and sword first.", true);
	try {
		const result = await request(
			`/StoreCharacters/${characterId}/equipment/${item.swordId}/${item.level}`,
			{ method: "PUT" },
		);
		showMessage(
			`${result.swordName} equipped (+${result.swordDamage} damage).`,
		);
		await refreshGame();
	} catch (exception) {
		showMessage(exception.message, true);
	}
});

async function chooseLoot(result) {
	const dialog = byId("loot-dialog");
	const root = byId("loot-choices");
	const autoLoot = byId("auto-loot");
	const countdown = byId("loot-countdown");
	let selectLoot;
	root.replaceChildren();
	autoLoot.checked = localStorage.getItem("dd-auto-loot") !== "false";
	for (const [index, item] of result.lootChoices.entries()) {
		const card = document.createElement("article");
		card.className = "loot-card";
		const art = document.createElement("pre");
		art.textContent = item.asciiArt;
		const name = document.createElement("strong");
		name.textContent = `${item.name} · LV ${item.level}`;
		const stats = document.createElement("p");
		stats.textContent =
			item.type === "sword"
				? `Sword · +${item.damage} damage`
				: `${item.slot} · ${item.defense} defense${item.swordDamageBonus ? ` · +${item.swordDamageBonus} sword damage` : ""}`;
		const character = api.characters.find(
			(hero) => hero.id === api.selectedCharacterId,
		);
		const equippedId =
			item.type === "sword"
				? character?.swordId
				: character?.[`${item.slot.toLowerCase()}Id`];
		const equippedLevel =
			item.type === "sword"
				? character?.swordLevel
				: character?.[`${item.slot.toLowerCase()}Level`];
		const equippedStatus = document.createElement("p");
		equippedStatus.className = "loot-equipped";
		equippedStatus.textContent =
			equippedId !== item.itemId
				? "Not currently equipped."
				: equippedLevel === item.level
					? `Currently equipped · LV ${equippedLevel}.`
					: `Same item equipped · LV ${equippedLevel}.`;
		const owned = api.inventory.filter(
			(gear) =>
				gear.type === item.type &&
				(item.type === "sword"
					? gear.swordId === item.itemId
					: gear.armorId === item.itemId),
		);
		const counts = new Map();
		for (const gear of owned)
			counts.set(
				gear.level,
				(counts.get(gear.level) || 0) + gear.quantity,
			);
		const ownership = document.createElement("p");
		ownership.className = "loot-owned";
		ownership.textContent = counts.size
			? `Owned: ${[...counts]
					.sort(([a], [b]) => a - b)
					.map(([level, quantity]) => `LV ${level} ×${quantity}`)
					.join(" · ")}`
			: "Owned: none";
		const nextLevel = item.level;
		const quantityAfterPick = (counts.get(nextLevel) || 0) + 1;
		const upgrade = document.createElement("p");
		upgrade.className = "loot-upgrade";
		upgrade.textContent =
			nextLevel >= 3
				? "Already at max level."
				: quantityAfterPick >= 3
					? `This pick upgrades it to LV ${nextLevel + 1}.`
					: `${3 - quantityAfterPick} more at LV ${nextLevel} to upgrade.`;
		const button = document.createElement("button");
		button.className = "primary-button";
		button.textContent = "Choose this item";
		button.onclick = () => selectLoot(index, false);
		card.append(
			art,
			name,
			stats,
			equippedStatus,
			ownership,
			upgrade,
			button,
		);
		root.append(card);
	}
	dialog.oncancel = (event) => event.preventDefault();
	dialog.showModal();
	return new Promise((resolve) => {
		let secondsLeft = 10;
		let timer;
		let claiming = false;
		const buttons = [...root.querySelectorAll("button")];
		const startCountdown = () => {
			clearInterval(timer);
			if (!autoLoot.checked) {
				countdown.textContent =
					"Auto-loot is off. Choose an item to continue the fight.";
				return;
			}
			secondsLeft = 10;
			countdown.textContent = `Random loot selected in ${secondsLeft} seconds.`;
			timer = setInterval(() => {
				secondsLeft--;
				if (secondsLeft <= 0) {
					clearInterval(timer);
					claimLoot(
						Math.floor(Math.random() * result.lootChoices.length),
						true,
					);
				} else
					countdown.textContent = `Random loot selected in ${secondsLeft} seconds.`;
			}, 1000);
		};
		const claimLoot = async (index, automatic) => {
			if (claiming) return;
			claiming = true;
			clearInterval(timer);
			buttons.forEach((button) => (button.disabled = true));
			try {
				await request(`/Fight/${result.lootOfferId}/loot`, {
					method: "POST",
					body: JSON.stringify({ choiceIndex: index }),
				});
				dialog.close();
				showMessage(
					`${automatic ? "Auto-picked" : "Chosen"}: ${result.lootChoices[index].name}.`,
				);
				await refreshGame();
				resolve();
			} catch (exception) {
				claiming = false;
				buttons.forEach((button) => (button.disabled = false));
				showMessage(exception.message, true);
				startCountdown();
			}
		};
		selectLoot = claimLoot;
		autoLoot.onchange = () => {
			localStorage.setItem("dd-auto-loot", String(autoLoot.checked));
			startCountdown();
		};
		startCountdown();
	});
}

async function loadMusic() {
	const select = byId("music-track");
	try {
		const response = await fetch("/audio/tracks.json", {
			cache: "no-store",
		});
		if (!response.ok) throw new Error("Track list unavailable");
		const tracks = await response.json();
		select.replaceChildren();
		if (!tracks.length)
			return addOption(select, "", "Add tracks to wwwroot/audio");
		for (const track of tracks)
			addOption(select, track.file, track.title);
		select.dispatchEvent(new Event("change"));
	} catch {
		select.replaceChildren();
		addOption(select, "", "No tracks configured");
	}
}

const audio = byId("game-music");
byId("music-track").addEventListener("change", (event) => {
	const file = event.target.value;
	audio.pause();
	audio.src = file ? `/audio/${encodeURIComponent(file)}` : "";
	byId("music-toggle").textContent = "▶";
});
byId("music-toggle").addEventListener("click", async () => {
	if (!audio.src)
		return showMessage(
			"Add your audio files and list them in wwwroot/audio/tracks.json.",
			true,
		);
	if (audio.paused) {
		try {
			await audio.play();
			byId("music-toggle").textContent = "Ⅱ";
		} catch {
			showMessage(
				"The browser could not play this audio file.",
				true,
			);
		}
	} else {
		audio.pause();
		byId("music-toggle").textContent = "▶";
	}
});
byId("music-volume").addEventListener("input", (event) => {
	audio.volume = Number(event.target.value);
});
audio.volume = Number(byId("music-volume").value);

loadMusic();
if (api.token && api.userName) setLoggedIn(api.token, api.userName);
